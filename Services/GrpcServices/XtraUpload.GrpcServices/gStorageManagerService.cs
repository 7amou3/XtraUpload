using Grpc.Core;
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
        public gStorageManagerService(IStorageClientProxy storageProxy, ICheckClientProxy checkClientProxy)
        {
            _storageProxy = storageProxy;
            _checkClientProxy = checkClientProxy;
        }
        public override async Task GetUploadOptions(IAsyncStreamReader<UploadOptsResponse> requestStream, IServerStreamWriter<UploadOptsRequest> responseStream, ServerCallContext context)
        {
            _storageProxy.UploadOptsRequested += uploadOptionsRequested;
            try
            {
                while (await requestStream.MoveNext())
                {
                    await foreach(var message in requestStream.ReadAllAsync())
                    {
                        string server = context.Host;
                        _storageProxy.SetUploadOptions(message.UploadOptions.Convert(), server);
                    }
                }
            }
            finally
            {
                _storageProxy.UploadOptsRequested -= uploadOptionsRequested;
            }

            async void uploadOptionsRequested(object sender, UploadOptsRequestedEventArgs e)
            {
                await responseStream.WriteAsync(new UploadOptsRequest() { ServerAddress = e.ServerAddress });
            }
        }

        public override async Task CheckConnectivity(IAsyncStreamReader<ConnectivityResponse> requestStream, IServerStreamWriter<ConnectivityRequest> responseStream, ServerCallContext context)
        {
            _checkClientProxy.StorageServerConnectivityRequested += ConnectivityRequested;
            try
            {
                while (await requestStream.MoveNext())
                {
                    await foreach (var message in requestStream.ReadAllAsync())
                    {
                        _checkClientProxy.SetConnectivityStatus(message.Status.Convert());
                    }
                }
            }
            finally
            {
                _checkClientProxy.StorageServerConnectivityRequested -= ConnectivityRequested;
            }
            
            async void ConnectivityRequested(object sender, StorageServerConnectivityEventArgs e)
            {
                await responseStream.WriteAsync(new ConnectivityRequest() { ServerAddress = e.ServerAddress });
            }
        }

    }
}
