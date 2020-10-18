using MediatR;
using XtraUpload.GrpcServices.Common;

namespace XtraUpload.Administration.Service.Common
{
    /// <summary>
    /// Get the remote storage server config
    /// </summary>
    public class GetStorageServerConfigQuery : IRequest<UploadOptionsResult>
    {
        public GetStorageServerConfigQuery(string serverAddress)
        {
            ServerAddress = serverAddress;
        }
        public string ServerAddress { get; }
    }
}
