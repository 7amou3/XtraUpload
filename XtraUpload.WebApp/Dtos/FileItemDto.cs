using System;

namespace XtraUpload.WebApp
{
    internal class FileItemDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool HasPassword { get; set; }
        public bool IsAvailableOnline { get; set; }
        public string Extension { get; set; }
        public int DownloadCount { get; set; }
        public long Size { get; set; }
        /// <summary>
        /// Time to wait before download link is generated
        /// </summary>
        public int WaitTime { get; set; }
        public bool UserLoggedIn { get; set; }
    }
}
