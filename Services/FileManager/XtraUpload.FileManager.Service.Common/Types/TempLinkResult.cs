using System;
using System.Collections.Generic;
using System.Text;
using XtraUpload.Domain;

namespace XtraUpload.FileManager.Service.Common
{
    public class TempLinkResult: OperationResult
    {
        public Download FileDownload { get; set; }
    }
}
