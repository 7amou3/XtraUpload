using MediatR;
using XtraUpload.Domain;

namespace XtraUpload.Administration.Service.Common
{
    /// <summary>
    /// Delete extension
    /// </summary>
    public class DeleteExtensionCommand :IRequest<OperationResult>
    {
        public DeleteExtensionCommand(int extensionId)
        {
            ExtensionId = extensionId;
        }
        public int ExtensionId { get; }
    }
}
