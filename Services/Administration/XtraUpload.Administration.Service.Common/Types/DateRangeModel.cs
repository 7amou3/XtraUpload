using System;

namespace XtraUpload.Administration.Service.Common
{
    public class DateRangeModel
    {
        public DateRangeModel(DateTime start, DateTime end)
        {
            Start = start;
            End = end;
        }
        public DateTime Start { get; }
        public DateTime End { get; }
    }
}
