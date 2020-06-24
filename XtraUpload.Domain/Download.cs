using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace XtraUpload.Domain
{
    public class Download
    {
        /// <summary>
        /// Download Id
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Id { get; set; }
        /// <summary>
        /// Download start time
        /// </summary>
        public DateTime StartedAt { get; set; }
        /// <summary>
        /// Ip @ of the requester
        /// </summary>
        public string IpAdress { get; set; }
        /// <summary>
        /// File Id
        /// </summary>
        public virtual string FileId { get; set; }
        /// <summary>
        /// Navigation property
        /// </summary>
        public virtual FileItem File { get; set; }
    }
}
