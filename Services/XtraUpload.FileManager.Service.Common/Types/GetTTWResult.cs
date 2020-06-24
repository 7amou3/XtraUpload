using XtraUpload.Domain;

namespace XtraUpload.FileManager.Service.Common
{
    public class GetTTWResult
    {
        /// <summary>
        /// The number of the downloads in progress for a given client
        /// </summary>
        public int TotalDownloads { get; set; }
        /// <summary>
        /// Time to wait when a download is in progress
        /// </summary>
        public int TimeToWait { get; set; }
    }
}
