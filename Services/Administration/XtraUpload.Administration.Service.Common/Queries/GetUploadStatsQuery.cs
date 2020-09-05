using MediatR;
using System;

namespace XtraUpload.Administration.Service.Common
{
    /// <summary>
    /// Get upload count grouped by the given period of time
    /// </summary>
    public class GetUploadStatsQuery : IRequest<AdminOverViewResult>
    {
        public GetUploadStatsQuery(DateTime start, DateTime end)
        {
            Range = new DateRangeModel(start, end);
        }
        public GetUploadStatsQuery(DateRangeModel range)
        {
            Range = new DateRangeModel(range.Start, range.End);
        }
        public DateRangeModel Range { get; }
    }
}
