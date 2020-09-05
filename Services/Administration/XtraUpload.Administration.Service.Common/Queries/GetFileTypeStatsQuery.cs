using MediatR;
using System;

namespace XtraUpload.Administration.Service.Common
{
    /// <summary>
    /// Get file type count grouped by the given period of time
    /// </summary>
    public class GetFileTypeStatsQuery : IRequest<AdminOverViewResult>
    {
        public GetFileTypeStatsQuery(DateTime start, DateTime end)
        {
            Range = new DateRangeModel(start, end);
        }
        public GetFileTypeStatsQuery(DateRangeModel range)
        {
            Range = new DateRangeModel(range.Start, range.End);
        }
        public DateRangeModel Range { get; }
    }
}
