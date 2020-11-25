using System.Collections.Generic;
using XtraUpload.Domain;

namespace XtraUpload.Setting.Service.Common
{
    public class LanguagesResult : OperationResult
    {
        public IEnumerable<Language> Languages { get; set; }
    }
}
