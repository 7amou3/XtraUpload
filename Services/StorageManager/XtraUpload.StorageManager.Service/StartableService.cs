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
    public class StartableService
    {
        const short RETRY_DELAY = 10000;
        readonly UrlsConfig _urls;
        readonly UploadOptions _uploadOpts;
        readonly ILogger<StartableService> _logger;
        readonly gStorageManager.gStorageManagerClient _storageClient;

        public StartableService(
            gStorageManager.gStorageManagerClient storageClient,
            IOptionsMonitor<UploadOptions> uploadOpts,
            IOptions<UrlsConfig> urls,
            ILogger<StartableService> logger)
        {
            _logger = logger;
            _urls = urls.Value;
            _uploadOpts = uploadOpts.CurrentValue;
            _storageClient = storageClient;
        }

        /// <summary>
        /// Start all tasks
        /// </summary>
        public void Start()
        {
            // Queu this task to run on background thread, and to free the caller
            Task.Run(() =>
            {
                Task.WaitAll(StartStorageServerCheck(), StartStorageServerConfigRetrieval());
            });
        }
        private async Task StartStorageServerCheck()
        {
            try
            {
                using (var call = _storageClient.CheckConnectivity())
                {
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
        private async Task StartStorageServerConfigRetrieval()
        {
            try
            {
                using (var call = _storageClient.GetUploadOptions())
                {
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
                                    ChunkSize = _uploadOpts.ChunkSize,
                                    Expiration = _uploadOpts.Expiration,
                                    UploadPath = _uploadOpts.UploadPath 
                                }
                            });
                        }
                    }

                    _logger.LogDebug("Disconnecting");
                    await call.RequestStream.CompleteAsync();
                }
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
                await StartStorageServerConfigRetrieval();
            }
        }

    }
}
