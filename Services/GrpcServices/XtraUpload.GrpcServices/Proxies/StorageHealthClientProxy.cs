using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XtraUpload.Domain;
using XtraUpload.GrpcServices.Common;

namespace XtraUpload.GrpcServices
{
    /// <summary>
    /// Check the health status (Storage space, memory, cpu..) of storage clients
    /// </summary>
    public class StorageHealthClientProxy : IStorageHealthClientProxy, IStorageHealthClientCommand, IDisposable
    {
        private const int EXPIRATION = 60 * 1000; // 60s
        private Timer _timer;
        /// <summary>
        /// Max time to wait before request timed out (in miliseconds)
        /// </summary>
        private const ushort WAIT_TIME = 10000;
        /// <summary>
        /// Thread sync 
        /// </summary>
        private readonly SemaphoreSlim _signal = new SemaphoreSlim(0, 1);
        /// <summary>
        /// Connectivity check instance
        /// </summary>
        private readonly ICheckClientProxy _connectivityCheck;
        /// <summary>
        /// Logger instance
        /// </summary>
        private readonly ILogger<UploadOptsClientProxy> _logger;
        /// <summary>
        /// List of storage servers health status
        /// </summary>
        private readonly List<StorageHealthResult> _serversHealth = new List<StorageHealthResult>();
        /// <summary>
        /// Lock instance
        /// </summary>
        private readonly object _lock = new object();
        private StorageHealthResult _storageHealth = new StorageHealthResult();

        /// <summary>
        /// Create new instance of <see cref="UploadOptsClientProxy"/>
        /// </summary>
        public StorageHealthClientProxy(ICheckClientProxy connectivityCheck, ILogger<UploadOptsClientProxy> logger)
        {
            _logger = logger;
            _connectivityCheck = connectivityCheck;
        }

        /// <summary>
        /// Event raised to read storage health
        /// </summary>
        public event EventHandler<ReadStorageHealthEventArgs> ReadStorageHealthRequested;
        /// <summary>
        /// Gets the current storage servers health status
        /// </summary>
        public IEnumerable<StorageHealthResult> GetServersHealth
        {
            get
            {
                lock(_lock)
                {
                    return _serversHealth;
                }
            }
        }
        /// <summary>
        /// Initialize the service
        /// </summary>
        public void Initialize()
        {
            _timer = new Timer(RunStorageHealthCheck, new CancellationToken(), 0, Timeout.Infinite);
        }

        /// <summary>
        /// Run the storage health check
        /// </summary>
        private async void RunStorageHealthCheck(object state)
        {
            try
            {
                List<StorageHealthResult> tmpServersHealth = new List<StorageHealthResult>();
                foreach (var server in _connectivityCheck.GetHealthyServers)
                {
                    var result = await ReadStorageServerHealth(server.Address);
                    if (result != null && result.State == OperationState.Success)
                    {
                        tmpServersHealth.Add(result);
                    }
                }
                lock(_lock)
                {
                    _serversHealth.Clear();
                    _serversHealth.AddRange(tmpServersHealth);
                }
            }
            catch (Exception _ex)
            {
                _logger.LogError("Internal error: " + _ex.Message);
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
        /// <summary>
        /// Read the storage server health status
        /// </summary>
        private async Task<StorageHealthResult> ReadStorageServerHealth(string serverAddress)
        {
            StorageHealthResult Result = new StorageHealthResult();
            // Raise event to call client storage server
            ReadStorageHealthRequested?.Invoke(this, new ReadStorageHealthEventArgs(serverAddress));
            if (await _signal.WaitAsync(WAIT_TIME))
            {
                lock(_lock)
                {
                    if (_storageHealth != null)
                    {
                        Result.StorageInfo = new StorageSpaceInfo
                        {
                            FreeDiskSpace = _storageHealth.StorageInfo.FreeDiskSpace,
                            UsedDiskSpace = _storageHealth.StorageInfo.UsedDiskSpace
                        };
                    }
                    else
                    {
                        _logger.LogError("No record found for the server: " + serverAddress);
                        Result.ErrorContent = new ErrorContent("No record found, please try again.", ErrorOrigin.Server);
                    }
                }
            }
            else
            {
                _logger.LogError("Response timed out for server: " + serverAddress);
                Result.ErrorContent = new ErrorContent("Timeout: Response not received, please check that the server is up and running.", ErrorOrigin.Server);
            }

            return Result;
        }

        /// <summary>
        /// Sets the storage server health status
        /// </summary>
        public void SetStorageHealthStatus(StorageHealthResult storageHealth, string callerAddress)
        {
            lock(_lock)
            {
                _storageHealth = storageHealth;
            }
            _signal.Release();

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
