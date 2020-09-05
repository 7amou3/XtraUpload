using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using XtraUpload.Administration.Service.Common;
using XtraUpload.Domain;

namespace XtraUpload.Administration.Service
{
    /// <summary>
    /// Get user count grouped by the given period of time
    /// </summary>
    public class GetUserStatsQueryHandler : IRequestHandler<GetUserStatsQuery, AdminOverViewResult>
    {
        readonly IMediator _mediatr;

        public GetUserStatsQueryHandler(IMediator mediatr)
        {
            _mediatr = mediatr;
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

            Result.UsersCount = await _mediatr.Send(new GetUsersHistoryQuery(request.Range));
            return Result;
        }
    }
}
