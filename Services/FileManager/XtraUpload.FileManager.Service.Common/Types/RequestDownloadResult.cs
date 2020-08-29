using System;
using System.Collections.Generic;
using System.Text;
using XtraUpload.Domain;

namespace XtraUpload.FileManager.Service.Common
{
    public class RequestDownloadResult : OperationResult
    {
        /// <summary>
        /// The file to download
        /// </summary>
        public FileItem File { get; set; }
        /// <summary>
        /// The time to wait before download process started
        /// </summary>
        public int WaitTime { get; set; }
    }
}
