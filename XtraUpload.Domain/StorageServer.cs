using System;
using System.Collections.Generic;

namespace XtraUpload.Domain
{
    /// <summary>
    /// Represent a storgae server
    /// </summary>
    public class StorageServer
    {
        public StorageServer()
        {
            Files = new HashSet<FileItem>();
        }
        /// <summary>
        /// Id of the server
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// Ip of the server
        /// </summary>
        public string IpAddress { get; set; }
        /// <summary>
        /// Navigation property
        /// </summary>
        public virtual ICollection<FileItem> Files { get; set; }
    }
}
