using System;
using System.Collections.Generic;
using System.Text;
using XtraUpload.Domain;

namespace XtraUpload.Administration.Service.Common
{
    public class FileExtensionsResult : OperationResult
    {
        public IEnumerable<FileExtension> FileExtensions { get; set; }
    }
}
