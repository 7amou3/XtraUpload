using XtraUpload.Domain;

namespace XtraUpload.Setting.Service.Common
{
    public class AccountOverviewResult : OperationResult
    {
        public UploadSettingResult UploadSetting { get; set; }
        public DownloadSettingResult DownloadSetting { get; set; }
        public FilesStatsResult FilesStats { get; set; }
        public User User { get; set; }
    }
}
