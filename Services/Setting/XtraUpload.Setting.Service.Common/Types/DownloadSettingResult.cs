namespace XtraUpload.Setting.Service.Common
{
    public class DownloadSettingResult
    {
        /// <summary>
        /// The max download speed (in bytes)
        /// </summary>
        public int DownloadSpeed { get; set; }
        /// <summary>
        /// Time to wait before download start (int seconds)
        /// </summary>
        public int TimeToWait { get; set; }
        /// <summary>
        /// File expiration time (expired files will be deleted) (int days)
        /// </summary>
        public int FileExpiration { get; set; }
        /// <summary>
        /// Time to wait before requesting a new download (int seconds)
        /// </summary>
        public int DownloadTTW { get; set; }
    }
}
