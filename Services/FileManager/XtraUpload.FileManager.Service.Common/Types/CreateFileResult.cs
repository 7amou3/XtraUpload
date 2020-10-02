using XtraUpload.Domain;

namespace XtraUpload.FileManager.Service.Common
{
    public class CreateFileResult: OperationResult
    {
        public CreateFileResult()
        {
            File = new FileItem();
        }
        public FileItem File { get; set; }
    }
}
