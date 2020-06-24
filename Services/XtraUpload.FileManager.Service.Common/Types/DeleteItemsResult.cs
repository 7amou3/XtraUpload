using System;
using System.Collections.Generic;
using System.Text;
using XtraUpload.Domain;

namespace XtraUpload.FileManager.Service.Common
{
    public class DeleteItemsResult : OperationResult
    {
        public IEnumerable<DeleteFolderResult> Folders { get; set; }
        public IEnumerable<FileItem> Files { get; set; }
    }
}
