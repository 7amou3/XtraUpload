using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XtraUpload.Administration.Service.Common;
using XtraUpload.Database.Data.Common;

namespace XtraUpload.Administration.Service
{
    public class GetUploadsHistoryQueryHandler : IRequestHandler<GetUploadsHistoryQuery, IEnumerable<ItemCountResult>>
    {
        readonly IUnitOfWork _unitOfWork;
        public GetUploadsHistoryQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IEnumerable<ItemCountResult>> Handle(GetUploadsHistoryQuery request, CancellationToken cancellationToken)
        {
            List<ItemCountResult> files = new List<ItemCountResult>(await _unitOfWork.Files.FilesCountByDateRange(request.Range.Start, request.Range.End));

            return AdministrationHelpers.FormatResult(request.Range, files);
        }
    }
}
