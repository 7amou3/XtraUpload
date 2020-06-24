using XtraUpload.Domain;

namespace XtraUpload.FileManager.Service.Common
{
    public class DeleteFileResult: OperationResult
    {
        public FileItem File { get; set; }
    }
}
