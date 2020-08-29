using System.Collections.Generic;
using XtraUpload.Domain;

namespace XtraUpload.FileManager.Service.Common
{
    public class GetFolderContentResult: OperationResult
    {
        public IEnumerable<FolderItem> Folders { get; set; }
        public IEnumerable<FileItem> Files { get; set; }
    }
}
