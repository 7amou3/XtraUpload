using MediatR;
using XtraUpload.Domain;

namespace XtraUpload.Administration.Service.Common
{
    /// <summary>
    /// Update a page
    /// </summary>
    public class UpdatePageCommand : IRequest<PageResult>
    {
        public UpdatePageCommand(Page page)
        {
            Page = page;
        }
        public Page Page { get; }
    }
}
