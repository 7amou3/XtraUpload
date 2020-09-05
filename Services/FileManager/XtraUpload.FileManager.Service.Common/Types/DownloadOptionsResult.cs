using XtraUpload.Domain;

namespace XtraUpload.FileManager.Service.Common
{
    /// <summary>
    /// Download client option
    /// </summary>
    public class DownloadOptionsResult : OperationResult
    {
        public double Speed { get; set; }
        public int TTW { get; set; }
        public int WaitTime { get; set; }
    }
}
