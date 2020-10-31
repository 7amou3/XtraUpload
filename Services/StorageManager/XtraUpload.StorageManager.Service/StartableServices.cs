using Grpc.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using XtraUpload.Domain;
using XtraUpload.Protos;
using XtraUpload.StorageManager.Common;

namespace XtraUpload.StorageManager.Service
{
    /// <summary>
    /// Starts and maintain a duplex connexion with the main server in order to retrieve live configuration, check connectivity..
    /// </summary>
    public class StartableServices
    {
        const short RETRY_DELAY = 10000;
        readonly UrlsConfig _urls;
        readonly IWritableOptions<UploadOptions> _uploadOpts;
        readonly IWritableOptions<HardwareCheckOptions> _hardwareOpts;
        readonly gStorageManager.gStorageManagerClient _storageClient;
        readonly ILogger<StartableServices> _logger;

        public StartableServices(
            gStorageManager.gStorageManagerClient storageClient,
            IWritableOptions<HardwareCheckOptions> hardwareOpts,
            IWritableOptions<UploadOptions> uploadOpts,
            IOptions<UrlsConfig> urls,
            ILogger<StartableServices> logger)
        {
            _logger = logger;
            _urls = urls.Value;
            _storageClient = storageClient;
            _uploadOpts = uploadOpts;
            _hardwareOpts = hardwareOpts;
        }

        /// <summary>
        /// Start all tasks
        /// </summary>
        public void Start()
        {
            // Free the caller and Queu this task to run on background thread
            Task.Run(() =>
            {
                Task.WaitAll(
                    StartStorageServerCheck(), 
                    StartUploadConfigRetrieval(),
                    StartUploadConfigWrite(),
                    StartHardwareConfigRetrieval(),
                    StartHardwareConfigWrite());
            });
        }
        private async Task StartStorageServerCheck()
        {
            try
            {
                using var call = _storageClient.CheckConnectivity();
                _logger.LogDebug("Start connection");
                await foreach (var message in call.ResponseStream.ReadAllAsync())
                {
                    if (_urls.ServerUrl == message.ServerAddress)
                    {
                        await call.RequestStream.WriteAsync(new ConnectivityResponse() { Status = new gRequestStatus() });
                    }
                }
                _logger.LogDebug("Disconnecting");
                await call.RequestStream.CompleteAsync();
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
            finally
            {
                _logger.LogError("Connexion lost, retrying to establish new connetion in progress...");
                await Task.Delay(RETRY_DELAY);
                // Retry new connection
                await StartStorageServerCheck();
            }
        }
        private async Task StartUploadConfigRetrieval()
        {
            try
            {
                using var call = _storageClient.GetUploadOptions();
                _logger.LogDebug("Start connection");

                await foreach (var message in call.ResponseStream.ReadAllAsync())
                {
                    if (_urls.ServerUrl == message.ServerAddress)
                    {
                        await call.RequestStream.WriteAsync(new UploadOptsResponse()
                        {
                            ServerAddress = _urls.ServerUrl,
                            UploadOptions = new gUploadOptions()
                            {
                                ChunkSize = _uploadOpts.Value.ChunkSize,
                                Expiration = _uploadOpts.Value.Expiration,
                                UploadPath = _uploadOpts.Value.UploadPath
                            }
                        });
                    }
                }

                _logger.LogDebug("Disconnecting");
                await call.RequestStream.CompleteAsync();
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
            finally
            {
                _logger.LogError("Connexion lost, retrying to establish new connetion in progress...");
                await Task.Delay(RETRY_DELAY);
                // Retry new connection
                await StartUploadConfigRetrieval();
            }
        }
        private async Task StartUploadConfigWrite()
        {
            try
            {
                using var call = _storageClient.SetUploadOptions();
                _logger.LogDebug("Start connection");

                await foreach (var message in call.ResponseStream.ReadAllAsync())
                {
                    if (_urls.ServerUrl == message.ServerAddress)
                    {
                        // Write the upload opts to appsettings
                        await _uploadOpts.Update(s =>
                        {
                            s.ChunkSize = message.UploadOptions.ChunkSize;
                            s.Expiration = message.UploadOptions.Expiration;
                            s.UploadPath = message.UploadOptions.UploadPath;
                        });
                        await call.RequestStream.WriteAsync(new UploadOptsResponse()
                        {
                            ServerAddress = _urls.ServerUrl,
                            UploadOptions = new gUploadOptions()
                            {
                                ChunkSize = message.UploadOptions.ChunkSize,
                                Expiration = message.UploadOptions.Expiration,
                                UploadPath = message.UploadOptions.UploadPath
                            }
                        });
                    }
                }

                _logger.LogDebug("Disconnecting");
                await call.RequestStream.CompleteAsync();
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
            finally
            {
                _logger.LogError("Connexion lost, retrying to establish new connetion in progress...");
                await Task.Delay(RETRY_DELAY);
                // Retry new connection
                await StartUploadConfigWrite();
            }
        }
        private async Task StartHardwareConfigRetrieval()
        {
            try
            {
                using var call = _storageClient.GetHardwareOptions();
                _logger.LogDebug("Start connection");

                await foreach (var message in call.ResponseStream.ReadAllAsync())
                {
                    if (_urls.ServerUrl == message.ServerAddress)
                    {
                        await call.RequestStream.WriteAsync(new HardwareOptsResponse()
                        {
                            ServerAddress = _urls.ServerUrl,
                            HardwareOptions = new gHardwareOptions()
                            {
                                MemoryThreshold = _hardwareOpts.Value.MemoryThreshold,
                                StorageThreshold = _hardwareOpts.Value.StorageThreshold
                            }
                        });
                    }
                }

                _logger.LogDebug("Disconnecting");
                await call.RequestStream.CompleteAsync();
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
            finally
            {
                _logger.LogError("Connexion lost, retrying to establish new connetion in progress...");
                await Task.Delay(RETRY_DELAY);
                // Retry new connection
                await StartHardwareConfigRetrieval();
            }
        }
        private async Task StartHardwareConfigWrite()
        {
            try
            {
                using var call = _storageClient.SetHardwareOptions();
                _logger.LogDebug("Start connection");

                await foreach (var message in call.ResponseStream.ReadAllAsync())
                {
                    if (_urls.ServerUrl == message.ServerAddress)
                    {
                        // Write the hardware opts to appsettings
                        await _hardwareOpts.Update(s =>
                        {
                            s.MemoryThreshold = (ushort)message.HardwareOptions.MemoryThreshold;
                            s.StorageThreshold = (ushort)message.HardwareOptions.StorageThreshold;
                        });
                        await call.RequestStream.WriteAsync(new HardwareOptsResponse()
                        {
                            ServerAddress = _urls.ServerUrl,
                            HardwareOptions = new gHardwareOptions()
                            {
                                StorageThreshold = message.HardwareOptions.StorageThreshold,
                                MemoryThreshold = message.HardwareOptions.MemoryThreshold
                            }
                        });
                    }
                }

                _logger.LogDebug("Disconnecting");
                await call.RequestStream.CompleteAsync();
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
            finally
            {
                _logger.LogError("Connexion lost, retrying to establish new connetion in progress...");
                await Task.Delay(RETRY_DELAY);
                // Retry new connection
                await StartHardwareConfigWrite();
            }
        }
    }
}
