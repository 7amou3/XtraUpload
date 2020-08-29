using XtraUpload.Domain;

namespace XtraUpload.FileManager.Service.Common
{
    public class FolderAvailabilityResult: OperationResult
    {
        public FolderItem Folder { get; set; }
    }
}
