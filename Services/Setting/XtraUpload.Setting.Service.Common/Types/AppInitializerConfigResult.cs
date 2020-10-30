using System.Collections.Generic;
using XtraUpload.Domain;

namespace XtraUpload.Setting.Service.Common
{
    public class AppInitializerConfigResult: OperationResult
    {
        public WebAppInfo AppInfo { get; set; }
        public IEnumerable<PageHeader> Pages { get; set; }
        public string Version { get; set; }
    }
}
