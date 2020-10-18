using MediatR;
using XtraUpload.GrpcServices.Common;

namespace XtraUpload.Administration.Service.Common
{
    /// <summary>
    /// Get the upload config of the remote storage server
    /// </summary>
    public class GetUploadConfigConfigQuery : IRequest<UploadOptionsResult>
    {
        public GetUploadConfigConfigQuery(string serverAddress)
        {
            ServerAddress = serverAddress;
        }
        public string ServerAddress { get; }
    }
}
