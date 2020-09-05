using MediatR;
using System;

namespace XtraUpload.Administration.Service.Common
{
    /// <summary>
    /// Get upload count grouped by the given period of time
    /// </summary>
    public class GetUploadCountsQuery : IRequest<AdminOverViewResult>
    {
        public GetUploadCountsQuery(DateTime start, DateTime end)
        {
            Range = new DateRangeModel(start, end);
        }
        public GetUploadCountsQuery(DateRangeModel range)
        {
            Range = new DateRangeModel(range.Start, range.End);
        }
        public DateRangeModel Range { get; }
    }
}
