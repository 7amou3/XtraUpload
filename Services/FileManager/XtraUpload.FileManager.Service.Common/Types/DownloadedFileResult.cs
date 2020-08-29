using XtraUpload.Domain;

namespace XtraUpload.FileManager.Service.Common
{
    public class DownloadedFileResult
    {
        public FileItem File { get; set; }
        public Download Download { get; set; }
    }
}
