using MediatR;
using System;

namespace XtraUpload.Administration.Service.Common
{
    /// <summary>
    /// An overview of the current stats (users, files, disk..)
    /// </summary>
    public class GetAdminOverViewQuery : IRequest<AdminOverViewResult>
    {
        public GetAdminOverViewQuery(DateTime start, DateTime end)
        {
            DateRange = new DateRangeModel(start, end);
        }
        public GetAdminOverViewQuery(DateRangeModel dateRange)
        {
            DateRange = new DateRangeModel(dateRange.Start, DateRange.End);
        }
        public DateRangeModel DateRange { get; }
    }
}
