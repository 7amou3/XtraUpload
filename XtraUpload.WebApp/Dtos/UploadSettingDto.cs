namespace XtraUpload.WebApp
{
    internal class UploadSettingDto
    {
        public int ConcurrentUpload { get; set; }
        public double StorageSpace { get; set; }
        public double UsedSpace { get; set; }
        public int MaxFileSize { get; set; }
    }
}
