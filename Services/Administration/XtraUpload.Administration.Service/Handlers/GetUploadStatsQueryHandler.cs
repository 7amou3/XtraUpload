using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XtraUpload.Administration.Service.Common;
using XtraUpload.Database.Data.Common;
using XtraUpload.Domain;

namespace XtraUpload.Administration.Service
{
    /// <summary>
    /// Get upload count grouped by the given period of time
    /// </summary>
    public class GetUploadStatsQueryHandler : IRequestHandler<GetUploadStatsQuery, AdminOverViewResult>
    {
        readonly IUnitOfWork _unitOfWork;
        public GetUploadStatsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<AdminOverViewResult> Handle(GetUploadStatsQuery request, CancellationToken cancellationToken)
        {
            AdminOverViewResult Result = new AdminOverViewResult();

            // Check date range is valid
            if (request.Range.Start.Date > request.Range.End.Date)
            {
                Result.ErrorContent = new ErrorContent("Invalid date range.", ErrorOrigin.Client);
                return Result;
            }

            // Query db
            List<ItemCountResult> files = new List<ItemCountResult>(await _unitOfWork.Files.FilesCountByDateRange(request.Range.Start, request.Range.End));

            Result.FilesCount = AdministrationHelpers.FormatResult(request.Range, files);
            return Result;
        }
    }
}
