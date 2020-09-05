using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XtraUpload.Administration.Service.Common;
using XtraUpload.Domain;

namespace XtraUpload.Administration.Service
{
    /// <summary>
    /// Get file type count grouped by the given period of time
    /// </summary>
    public class GetFileTypeStatsQueryHandler : IRequestHandler<GetFileTypeStatsQuery, AdminOverViewResult>
    {
        readonly IMediator _mediatr;

        public GetFileTypeStatsQueryHandler(IMediator mediatr)
        {
            _mediatr = mediatr;
        }

        public async Task<AdminOverViewResult> Handle(GetFileTypeStatsQuery request, CancellationToken cancellationToken)
        {
            AdminOverViewResult Result = new AdminOverViewResult();
            // Check date range is valid
            if (request.Range.Start.Date > request.Range.End.Date)
            {
                Result.ErrorContent = new ErrorContent("Invalid date range.", ErrorOrigin.Client);
                return Result;
            }

            Result.FileTypesCount = await _mediatr.Send(new GetFileTypesCountQuery(request.Range));
            return Result;
        }
    }
}
