using MediatR;
using System.Threading;
using System.Threading.Tasks;
using XtraUpload.Administration.Service.Common;
using XtraUpload.Database.Data.Common;

namespace XtraUpload.Administration.Service
{
    /// <summary>
    /// Get all pages
    /// </summary>
    public class GetPagesQueryHandler : IRequestHandler<GetPagesQuery, PagesResult>
    {
        readonly IUnitOfWork _unitOfWork;
        public GetPagesQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<PagesResult> Handle(GetPagesQuery request, CancellationToken cancellationToken)
        {
            return new PagesResult()
            {
                Pages = await _unitOfWork.Pages.GetAll()
            };
        }
    }
}
