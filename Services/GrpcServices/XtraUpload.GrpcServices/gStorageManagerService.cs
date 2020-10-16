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
        readonly IStorageClientProxy _proxy;
        public gStorageManagerService(IStorageClientProxy proxy)
        {
            _proxy = proxy;
        }
        public override async Task GetUploadOptions(IAsyncStreamReader<UploadOptsResponse> requestStream, IServerStreamWriter<UploadOptsRequest> responseStream, ServerCallContext context)
        {
            _proxy.UploadOptsRequested += uploadOptionsRequested;
            try
            {
                while (await requestStream.MoveNext())
                {
                    await foreach(var message in requestStream.ReadAllAsync())
                    {
                        string server = context.Host;
                        _proxy.SetUploadOptions(message.UploadOptions.Convert(), server);
                    }
                }
            }
            finally
            {
                _proxy.UploadOptsRequested -= uploadOptionsRequested;
            }

            async void uploadOptionsRequested(object sender, UploadOptsRequestedEventArgs e)
            {
                await responseStream.WriteAsync(new UploadOptsRequest() { ServerAddress = e.ServerAddress });
            }
        }

    }
}
