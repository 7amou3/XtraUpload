namespace XtraUpload.WebApp
{
    internal class AccountOverviewDto
    {
        public UploadSettingDto UploadSetting { get; set; }
        public DownloadSettingDto DownloadSetting { get; set; }
        public FilesStatsDto FilesStats { get; set; }
        public UserDto User { get; set; }
    }
}
