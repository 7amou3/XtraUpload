using MediatR;
using System;

namespace XtraUpload.Administration.Service.Common
{
    /// <summary>
    /// Get user count grouped by the given period of time
    /// </summary>
    public class GetUserStatsQuery : IRequest<AdminOverViewResult>
    {
        public GetUserStatsQuery(DateTime start, DateTime end)
        {
            Range = new DateRangeModel(start, end);
        }
        public GetUserStatsQuery(DateRangeModel range)
        {
            Range = new DateRangeModel(range.Start, range.End);
        }
        public DateRangeModel Range { get; }
    }
}
