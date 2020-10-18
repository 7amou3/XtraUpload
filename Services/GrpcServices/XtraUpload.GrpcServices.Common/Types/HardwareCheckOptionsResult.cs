using System;
using System.Collections.Generic;
using System.Text;
using XtraUpload.Domain;

namespace XtraUpload.GrpcServices.Common
{
    public class HardwareCheckOptionsResult : OperationResult
    {
        public HardwareCheckOptions HardwareOptions { get; set; }
    }
}
