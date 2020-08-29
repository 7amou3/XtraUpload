using XtraUpload.Domain;

namespace XtraUpload.FileManager.Service.Common
{
    public class RenameFolderResult: OperationResult
    {
        public FolderItem Folder { get; set; }
    }
}
