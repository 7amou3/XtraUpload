using System;
using System.Collections.Generic;
using System.Text;
using XtraUpload.Domain;

namespace XtraUpload.Administration.Service.Common
{
    public class DeleteFilesResult : OperationResult
    {
        public IEnumerable<FileItem> Files { get; set; }
    }
}
