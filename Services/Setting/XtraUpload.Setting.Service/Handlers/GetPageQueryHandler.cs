using MediatR;
using System.Threading;
using System.Threading.Tasks;
using XtraUpload.Administration.Service.Common;
using XtraUpload.Database.Data.Common;
using XtraUpload.Domain;
using XtraUpload.Setting.Service.Common;

namespace XtraUpload.Setting.Service
{
    /// <summary>
    /// Get a page by name
    /// </summary>
    public class GetPageQueryHandler : IRequestHandler<GetPageQuery, PageResult>
    {
        readonly IUnitOfWork _unitOfWork;
        
        public GetPageQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        
        public async Task<PageResult> Handle(GetPageQuery request, CancellationToken cancellationToken)
        {
            PageResult result = new PageResult();
            Page page = await _unitOfWork.Pages.FirstOrDefaultAsync(s => s.Url.ToLower() == request.PageName);
            if (page == null)
            {
                result.ErrorContent = new ErrorContent("Page not found.", ErrorOrigin.Client);
                return result;
            }

            result.Page = page;
            return result;
        }
    }
}
