using MediatR;
using XtraUpload.Domain;

namespace XtraUpload.Administration.Service.Common
{
    public class CheckStorageServerConnectivityQuery : IRequest<OperationResult>
    {
        public CheckStorageServerConnectivityQuery(string serverAddress)
        {
            ServerAddress = serverAddress;
        }
        public string ServerAddress { get; }
    }
}
