using System;
using System.ComponentModel.DataAnnotations;

namespace XtraUpload.WebApp
{
    public class DateRangeViewModel
    {
        [DataType(DataType.DateTime)]
        public DateTime Start { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime End 
        { 
            get { return _end; }
            set { _end = value.Date.AddHours(23).AddMinutes(59).AddSeconds(59); }
        }
        private DateTime _end;
    }
}
