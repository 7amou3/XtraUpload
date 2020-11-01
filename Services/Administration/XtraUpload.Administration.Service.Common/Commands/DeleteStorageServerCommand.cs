using MediatR;

namespace XtraUpload.Administration.Service.Common
{
    public class DeleteStorageServerCommand : IRequest<StorageServerResult>
    {
        public DeleteStorageServerCommand(string serverId)
        {
            ServerId = serverId;
        }
        public string ServerId { get; }
    }
}
