using System;

namespace XtraUpload.WebApi
{
    internal class FolderItemDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastModified { get; set; }
        public bool HasPassword { get; set; }
        public bool IsAvailableOnline { get; set; }        
        public string Parentid { get; set; }
    }
}
