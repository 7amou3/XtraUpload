using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using XtraUpload.Administration.Service.Common;
using XtraUpload.Database.Data.Common;
using XtraUpload.Domain;

namespace XtraUpload.Administration.Service
{
    /// <summary>
    /// Get user count grouped by the given period of time
    /// </summary>
    public class GetUserStatsQueryHandler : IRequestHandler<GetUserStatsQuery, AdminOverViewResult>
    {
        readonly IUnitOfWork _unitOfWork;

        public GetUserStatsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<AdminOverViewResult> Handle(GetUserStatsQuery request, CancellationToken cancellationToken)
        {
            AdminOverViewResult Result = new AdminOverViewResult();

            // Check date range is valid
            if (request.Range.Start.Date > request.Range.End.Date)
            {
                Result.ErrorContent = new ErrorContent("Invalid date range.", ErrorOrigin.Client);
                return Result;
            }
            // Get from db
            List<ItemCountResult> users = new List<ItemCountResult>(await _unitOfWork.Users.UsersCountByDateRange(request.Range.Start, request.Range.End));

            Result.UsersCount = AdministrationHelpers.FormatResult(request.Range, users);
            return Result;
        }
    }
}
