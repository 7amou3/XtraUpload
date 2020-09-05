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
    /// Get upload count grouped by the given period of time
    /// </summary>
    public class GetUploadCountsQueryHandler : IRequestHandler<GetUploadCountsQuery, AdminOverViewResult>
    {
        readonly IMediator _mediatr;
        public GetUploadCountsQueryHandler(IMediator mediatr)
        {
            _mediatr = mediatr;
        }
        public async Task<AdminOverViewResult> Handle(GetUploadCountsQuery request, CancellationToken cancellationToken)
        {
            AdminOverViewResult Result = new AdminOverViewResult();
            // Check date range is valid
            if (request.Range.Start.Date > request.Range.End.Date)
            {
                Result.ErrorContent = new ErrorContent("Invalid date range.", ErrorOrigin.Client);
                return Result;
            }

            Result.FilesCount = await _mediatr.Send( new GetUploadsHistoryQuery(request.Range));
            return Result;
        }
    }
}
