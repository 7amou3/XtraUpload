using System;
using System.ComponentModel.DataAnnotations;

namespace XtraUpload.ServerApp.Common
{
    public class HardwareCheckOptions
    {
        /// <summary>
        /// The maximum RAM memory that could be allocated by XtraUpload application (in gb)
        /// </summary>
        [Range(1, int.MaxValue)]
        public int MemoryThreshold { get; set; }
        /// <summary>
        /// The maximum disk space that could be allocated by XtraUpload (in gb)
        /// </summary>
        [Range(1, int.MaxValue)]
        public int StorageThreshold { get; set; }
    }
}
