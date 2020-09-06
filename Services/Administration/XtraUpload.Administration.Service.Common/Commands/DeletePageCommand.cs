using MediatR;
using XtraUpload.Domain;

namespace XtraUpload.Administration.Service.Common
{
    /// <summary>
    /// Delete a Page
    /// </summary>
    public class DeletePageCommand : IRequest<OperationResult>
    {
        public DeletePageCommand(string pageId)
        {
            PageId = pageId;
        }
        public string PageId { get; set; }
    }
}
