using MediatR;
using System.Threading;
using System.Threading.Tasks;
using XtraUpload.Administration.Service.Common;
using XtraUpload.Database.Data.Common;

namespace XtraUpload.Administration.Service
{
    /// <summary>
    /// Get all pages header (no page content is retrieved)
    /// </summary>
    public class GetPagesHeaderQueryHandler : IRequestHandler<GetPagesHeaderQuery, PagesHeaderResult>
    {
        readonly IUnitOfWork _unitOfWork;
        public GetPagesHeaderQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<PagesHeaderResult> Handle(GetPagesHeaderQuery request, CancellationToken cancellationToken)
        {
            return new PagesHeaderResult()
            {
                PagesHeader = await _unitOfWork.Pages.GetPagesHeader(request.Predicate)
            };
        }
    }
}
