using System.Collections.Generic;
using XtraUpload.Domain;

namespace XtraUpload.FileManager.Service.Common
{
    public class GetFoldersResult: OperationResult
    {
        public IEnumerable<FolderItem> Folders { get; set; }
    }
}
