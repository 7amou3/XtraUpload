using System.Threading.Tasks;
using XtraUpload.Domain;
using XtraUpload.FileManager.Service.Common;

namespace XtraUpload.Database.Data.Common
{
    public interface IDownloadRepository : IRepository<Download>
    {
        /// <summary>
        /// Get file by download id
        /// </summary>
        Task<DownloadedFileResult> GetDownloadedFile(string downloadId);
    }
}
