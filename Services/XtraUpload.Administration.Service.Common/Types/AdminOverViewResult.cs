using System;
using System.Collections.Generic;
using System.Text;
using XtraUpload.Domain;

namespace XtraUpload.Administration.Service.Common
{
    public class AdminOverViewResult : OperationResult
    {
        public int TotalUsers { get; set; }
        public int TotalFiles { get; set; }

        public long DriveSize { get; set; }
        public long FreeSpace { get; set; }

        public IEnumerable<ItemCountResult> FilesCount { get; set; }
        public IEnumerable<ItemCountResult> UsersCount { get; set; }
        public IEnumerable<FileTypeResult> FileTypesCount { get; set; }
    }
}
