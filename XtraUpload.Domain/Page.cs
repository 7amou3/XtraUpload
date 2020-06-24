using System;

namespace XtraUpload.Domain
{
    public class Page
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
