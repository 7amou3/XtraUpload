using XtraUpload.Domain;

namespace XtraUpload.FileManager.Service.Common
{
    public class GetFileResult: OperationResult
    {
        public FileItem File { get; set; }
    }
}
