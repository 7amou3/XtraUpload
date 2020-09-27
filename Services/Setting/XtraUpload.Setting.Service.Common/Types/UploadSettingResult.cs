using XtraUpload.Domain;

namespace XtraUpload.Setting.Service.Common
{
    public class UploadSettingResult: OperationResult
    {
        /// <summary>
        /// Concurent allowed uploads
        /// </summary>
        public int ConcurrentUpload { get; set; }
        /// <summary>
        /// Max storage space (in Mb)
        /// </summary>
        public double StorageSpace { get; set; }
        /// <summary>
        /// Total used space (in bytes)
        /// </summary>
        public double UsedSpace { get; set; }
        /// <summary>
        /// Allowed file size (in Mb)
        /// </summary>
        public int MaxFileSize { get; set; }
        /// <summary>
        /// The maximum file's chunk size (in bytes)
        /// </summary>
        public int ChunkSize { get; set; }
        /// <summary>
        /// The allowed file extensions to be uploaded (seperated by a ,)
        /// </summary>
        public string FileExtensions { get; set; }
        /// <summary>
        /// The upload server @ ip (or domain name)
        /// </summary>
        public StorageServer StorageServer { get; set; }
    }

}
