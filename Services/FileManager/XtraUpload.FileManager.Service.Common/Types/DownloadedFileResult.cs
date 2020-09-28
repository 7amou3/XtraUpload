using XtraUpload.Domain;

namespace XtraUpload.FileManager.Service.Common
{
    public class DownloadedFileResult : OperationResult
    {
        public FileItem File { get; set; }
        public Download Download { get; set; }
        public double DownloadSpeed { get; set; }
    }
}
