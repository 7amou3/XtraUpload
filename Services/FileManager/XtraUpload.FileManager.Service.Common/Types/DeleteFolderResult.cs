using System.Collections.Generic;
using XtraUpload.Domain;

namespace XtraUpload.FileManager.Service.Common
{
    public class DeleteFolderResult: OperationResult
    {
        public DeleteFolderResult()
        {
            Folders = new HashSet<FolderItem>();
            Files = new HashSet<FileItem>();
        }
        public IEnumerable<FolderItem> Folders { get; set; }
        public IEnumerable<FileItem> Files { get; set; }
    }
}
