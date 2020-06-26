using System;
using System.ComponentModel.DataAnnotations;

namespace XtraUpload.WebApp.Common
{
    public class FolderViewModel
    {
        [Required]
        [RegularExpression("^[a-zA-Z0-9]*$")]
        public string Id { get; set; }
        [Required]
        [MinLength(4)]
        public string Name { get; set; }
        public string Thumbnail { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastModified { get; set; }
        public bool HasPassword { get; set; }
        public bool IsAvailableOnline { get; set; }
        public string Parentid { get; set; }
    }
}
