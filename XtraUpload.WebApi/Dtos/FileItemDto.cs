using System;

namespace XtraUpload.WebApi
{
    internal class FileItemDto : FileItemHeaderDto
    {
        public DateTime CreatedAt { get; set; }
        public DateTime LastModified { get; set; }
        public bool IsAvailableOnline { get; set; }
        public int DownloadCount { get; set; }
        /// <summary>
        /// Time to wait before download link is generated
        /// </summary>
        public int WaitTime { get; set; }
        public bool UserLoggedIn { get; set; }
    }
}
