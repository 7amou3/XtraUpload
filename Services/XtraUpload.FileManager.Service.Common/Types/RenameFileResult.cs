using XtraUpload.Domain;

namespace XtraUpload.FileManager.Service.Common
{
    public class RenameFileResult: OperationResult
    {
        public FileItem File { get; set; }
    }
}
