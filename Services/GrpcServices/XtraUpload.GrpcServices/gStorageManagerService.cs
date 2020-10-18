using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XtraUpload.GrpcServices.Common;
using XtraUpload.Protos;

namespace XtraUpload.GrpcServices
{
    /// <summary>
    /// Service definition for storage service, server config, monitoring state...
    /// </summary>
    public class gStorageManagerService : gStorageManager.gStorageManagerBase
    {
        readonly IStorageClientProxy _storageProxy;
        readonly ICheckClientProxy _checkClientProxy;
        readonly ILogger<gStorageManagerService> _logger;
        public gStorageManagerService(IStorageClientProxy storageProxy, ICheckClientProxy checkClientProxy, ILogger<gStorageManagerService> logger)
        {
            _logger = logger;
            _storageProxy = storageProxy;
            _checkClientProxy = checkClientProxy;
        }
        
        public override async Task CheckConnectivity(IAsyncStreamReader<ConnectivityResponse> requestStream, IServerStreamWriter<ConnectivityRequest> responseStream, ServerCallContext context)
        {
            _checkClientProxy.StorageServerConnectivityRequested += ConnectivityRequested;
            try
            {
                await foreach (var message in requestStream.ReadAllAsync())
                {
                    string server = context.Host;
                    _logger.LogDebug("Request received from " + server);

                    _checkClientProxy.SetConnectivityStatus(message.Status.Convert());
                }
            }
            finally
            {
                _logger.LogError("Connection lost with the remote storage server");
                _checkClientProxy.StorageServerConnectivityRequested -= ConnectivityRequested;
            }
            
            async void ConnectivityRequested(object sender, StorageServerConnectivityEventArgs e)
            {
                _logger.LogDebug("Sending request to " + e.ServerAddress);

                await responseStream.WriteAsync(new ConnectivityRequest() { ServerAddress = e.ServerAddress });
            }
        }

        public override async Task GetUploadOptions(IAsyncStreamReader<UploadOptsResponse> requestStream, IServerStreamWriter<UploadOptsRequest> responseStream, ServerCallContext context)
        {
            _storageProxy.UploadOptsRequested += uploadOptionsRequested;
            try
            {
                
                await foreach (var message in requestStream.ReadAllAsync())
                {
                    _logger.LogDebug("Request received from " + message.ServerAddress);

                    _storageProxy.SetUploadOptions(message.UploadOptions.Convert(), message.ServerAddress);
                }
                
            }
            finally
            {
                _logger.LogError("Connection lost with the remote storage server");
                _storageProxy.UploadOptsRequested -= uploadOptionsRequested;
            }

            async void uploadOptionsRequested(object sender, UploadOptsRequestedEventArgs e)
            {
                _logger.LogDebug("Sending request to " + e.ServerAddress);

                await responseStream.WriteAsync(new UploadOptsRequest() { ServerAddress = e.ServerAddress });
            }
        }

    }
}
