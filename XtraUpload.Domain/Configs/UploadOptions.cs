using System;
using System.ComponentModel.DataAnnotations;

namespace XtraUpload.Domain
{
    /// <summary>
    /// Upload configuration options
    /// </summary>
    public class UploadOptions
    {
        /// <summary>
        /// The full path to the upload folder (must be writable)
        /// </summary>
        [Required]
        public string UploadPath { get; set; }
        /// <summary>
        /// The maximum size of a the uploaded file's chunk, must be less than the limit of your server request body size (in Mb)
        /// </summary>
        [Range(1, int.MaxValue)]
        public int ChunkSize { get; set; }
        /// <summary>
        /// Expiration time where incomplete files can no longer be updated (in minutes) 
        /// </summary>
        [Range(1, int.MaxValue)]
        public int Expiration { get; set; }
    }
}
