using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XtraUpload.Domain;
using XtraUpload.GrpcServices.Common;

namespace XtraUpload.GrpcServices
{
    public class CheckClientProxy : ICheckClientProxy
    {
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
        /// Logger instance
        /// </summary>
        private readonly ILogger<CheckClientProxy> _logger;
        /// <summary>
        /// Create new instance of <see cref="CheckClientProxy"/>
        /// </summary>
        public CheckClientProxy(ILogger<CheckClientProxy> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Event raised to check the client storage connectivity
        /// </summary>
        public event EventHandler<StorageServerConnectivityEventArgs> StorageServerConnectivityRequested;

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
    }
}
