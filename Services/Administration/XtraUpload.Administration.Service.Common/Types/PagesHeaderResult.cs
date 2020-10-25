using System.Collections.Generic;
using XtraUpload.Domain;

namespace XtraUpload.Administration.Service.Common
{
    /// <summary>
    /// Get all pages header (no page content is retrieved)
    /// </summary>
    public class PagesHeaderResult: OperationResult
    {
        public IEnumerable<PageHeader> PagesHeader { get; set; }
    }
}
