using XtraUpload.Domain;

namespace XtraUpload.FileManager.Service.Common
{
    public class FileAvailabilityResult: OperationResult
    {
        public FileItem File { get; set; }
    }
}
