using XtraUpload.Domain;

namespace XtraUpload.Setting.Service.Common
{
    public class FilesStatsResult: OperationResult
    {
        public int TotalFiles { get; set; }
        public double TotalDownloads { get; set; }
    }
}
