using System;
using System.ComponentModel.DataAnnotations;

namespace XtraUpload.Domain
{
    public class HardwareCheckOptions
    {
        /// <summary>
        /// The maximum RAM memory that could be allocated by XtraUpload application (in gb)
        /// </summary>
        [Range(1, ushort.MaxValue)]
        public ushort MemoryThreshold { get; set; }
        /// <summary>
        /// The maximum disk space that could be allocated by XtraUpload (in gb)
        /// </summary>
        [Range(1, ushort.MaxValue)]
        public ushort StorageThreshold { get; set; }
    }
}
