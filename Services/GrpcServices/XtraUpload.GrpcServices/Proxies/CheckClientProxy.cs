using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XtraUpload.Database.Data.Common;
using XtraUpload.Domain;
using XtraUpload.GrpcServices.Common;

namespace XtraUpload.GrpcServices
{
    /// <summary>
    /// Check the connectivity of storage clients
    /// </summary>
    public class CheckClientProxy : ICheckClientProxy, ICheckClientCommand, IDisposable
    {
        private const int EXPIRATION = 60 * 1000; // 1min
        private Timer _timer;
        private OperationResult _connectivityStatus = new OperationResult();
        /// <summary>
        /// Max time to wait before request timed out (in miliseconds)
        /// </summary>
        private const ushort WAIT_TIME = 5000;
        /// <summary>
        /// Thread sync 
        /// </summary>
        private readonly SemaphoreSlim _signal = new SemaphoreSlim(0, 1);
        /// <summary>
        /// ServiceProvider instance
        /// </summary>
        private readonly IServiceProvider _serviceProvider;
        /// <summary>
        /// Logger instance
        /// </summary>
        private readonly ILogger<CheckClientProxy> _logger;
        /// <summary>
        /// List of healthy servers
        /// </summary>
        private readonly List<StorageServer> _healthyServers = new List<StorageServer>();
        /// <summary>
        /// lock
        /// </summary>
        private readonly object _syncLock = new object();
        /// <summary>
        /// Create new instance of <see cref="CheckClientProxy"/>
        /// </summary>
        public CheckClientProxy(IServiceProvider serviceProvider, ILogger<CheckClientProxy> logger)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Event raised to check the client storage connectivity
        /// </summary>
        public event EventHandler<StorageServerConnectivityEventArgs> StorageServerConnectivityRequested;

        /// <summary>
        /// Initialize the service
        /// </summary>
        public void Initialize()
        {
            _timer = new Timer(RunServersCheck, new CancellationToken(), 0, Timeout.Infinite);
        }

        /// <summary>
        /// Check the storage server connectivity
        /// </summary>
        public async Task<StorageServerConnectivityResult> CheckServerStorageConnectivity(string serverAddress)
        {
            StorageServerConnectivityResult Result = new StorageServerConnectivityResult();

            if (StorageServerConnectivityRequested != null)
            {
                StorageServerConnectivityRequested.Invoke(this, new StorageServerConnectivityEventArgs(serverAddress));
                if (await _signal.WaitAsync(WAIT_TIME))
                {
                    Result = OperationResult.CopyResult<StorageServerConnectivityResult>(_connectivityStatus);
                }
                else
                {
                    _logger.LogError("Response timed out for server: " + serverAddress);
                    Result.ErrorContent = new ErrorContent("Timeout: Response not received, please check that the server is up and running.", ErrorOrigin.Server);
                }
            }
            else
            {
                Result.ErrorContent = new ErrorContent("No storage client is connected.", ErrorOrigin.Server);
            }
            
            return Result;
        }
        /// <summary>
        /// Sets the connectivity status of the given storage server (must be called only in the GrpcServices project)
        /// </summary>
        public void SetConnectivityStatus(OperationResult result)
        {
            _connectivityStatus = result;
            _signal.Release();
        }

        /// <summary>
        /// A list of healthy servers
        /// </summary>
        public IEnumerable<StorageServer> GetHealthyServers
        {
            get 
            { 
                lock (_syncLock) 
                {
                    return _healthyServers; 
                } 
            }
        }

        /// <summary>
        /// Check if storage servers are up and running
        /// </summary>
        /// <param name="state"></param>
        private async void RunServersCheck(object state)
        {
            try
            {
                _logger.LogInformation("Servers health check started");

                List<StorageServer> tempServers = new List<StorageServer>();
                using var scope = _serviceProvider.CreateScope();
                IUnitOfWork unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                IEnumerable<StorageServer> servers = await unitOfWork.StorageServer.FindAsync(s => s.State == ServerState.Active);
                if (servers.Any())
                {
                    foreach (StorageServer server in servers)
                    {
                        var response = await CheckServerStorageConnectivity(server.Address);
                        if (response != null && response.State == OperationState.Success)
                        {
                            tempServers.Add(server);
                            _logger.LogInformation($"Server {server.Address} is healthy");
                        }
                        else
                        {
                            _logger.LogWarning($"Server {server.Address} is unhealthy.");
                        }
                    }
                    // Add the storage servers result
                    lock (_syncLock)
                    {
                        _healthyServers.Clear();
                        _healthyServers.AddRange(tempServers);
                    }
                    if (!_healthyServers.Any())
                    {
                        _logger.LogError("HealthCheck error: No storage server could be reached.");
                    }
                }
                else
                {
                    _logger.LogWarning("Database error: No [Active] storage server found, please activate at least one server to allow uploads.");
                }
            }
            catch (Exception _ex)
            {
                _logger.LogError("Internal error: "+ _ex.Message);
            }
            finally
            {
                if (!((CancellationToken)state).IsCancellationRequested)
                {
                    // Re-schedule timer
                    _timer.Change(EXPIRATION, Timeout.Infinite);
                }
            }
        }

        public void Dispose()
        {
            if (_timer != null)
            {
                _timer.Change(Timeout.Infinite, Timeout.Infinite);
                _timer.Dispose();
                _timer = null;
            }
        }
    }
}
