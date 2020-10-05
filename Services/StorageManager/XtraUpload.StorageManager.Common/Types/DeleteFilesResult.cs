using System.Collections.Generic;
using XtraUpload.Domain;
using XtraUpload.Protos;

namespace XtraUpload.StorageManager.Common
{
    public class DeleteFilesResult : OperationResult
    {
        public IEnumerable<gFileItem> Files { get; set; }
    }
}
