using MediatR;
using XtraUpload.Domain;

namespace XtraUpload.Administration.Service.Common
{
    /// <summary>
    /// Add a new page
    /// </summary>
    public class AddPageCommand : IRequest<PageResult>
    {
        public AddPageCommand(Page page)
        {
            Page = page;
        }
        public Page Page { get; }
    }
}
