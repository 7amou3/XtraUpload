using System.Collections.Generic;
using XtraUpload.Domain;

namespace XtraUpload.Administration.Service.Common
{
    public class SearchUserResult : OperationResult
    {
        public IEnumerable<User> Users { get; set; }
    }
}
