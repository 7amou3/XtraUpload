using System.Threading.Tasks;

namespace XtraUpload.FileManager.Service.Common
{
    public interface IFileDownloadService
    {
        Task<StartDownloadResult> StartDownload(string downloadId);
    }
}
