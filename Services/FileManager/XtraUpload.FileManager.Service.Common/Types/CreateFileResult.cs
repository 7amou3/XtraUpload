using XtraUpload.Domain;

namespace XtraUpload.FileManager.Service.Common
{
    public class CreateFileResult: OperationResult
    {
        public FileItem File { get; set; }
    }
}
