using System.Threading.Tasks;

namespace XtraUpload.FileManager.Service.Common
{
    public interface IFileDownloadService
    {
        Task<RequestDownloadResult> RequestDownload(string fileid);
        Task<StartDownloadResult> StartDownload(string downloadId);
        Task<TempLinkResult> TempLink(string fileId);
    }
}
