using System;

namespace XtraUpload.Domain
{
    public class PageHeader
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public bool VisibleInFooter { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
    public class Page : PageHeader
    {
        public string Content { get; set; }

    }
}
