using MediatR;

namespace XtraUpload.FileManager.Service.Common
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
