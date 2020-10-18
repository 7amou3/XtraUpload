using System.Collections.Generic;
using XtraUpload.Domain;

namespace XtraUpload.Administration.Service.Common
{
    public class PagesResult: OperationResult
    {
        public IEnumerable<Page> Pages { get; set; }
    }
}
