using MediatR;

namespace XtraUpload.StorageManager.Common
{
    /// <summary>
    /// Start download a file 
    /// </summary>
    public class StartDownloadCommand : IRequest<StartDownloadResult>
    {
        public StartDownloadCommand(string downloadId)
        {
            DownloadId = downloadId;
        }
        public string DownloadId { get; set; }
    }
}
