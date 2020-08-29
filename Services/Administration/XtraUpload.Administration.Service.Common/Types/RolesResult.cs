using System.Collections.Generic;
using XtraUpload.Authentication.Service.Common;
using XtraUpload.Domain;

namespace XtraUpload.Administration.Service.Common
{
    public class RolesResult : OperationResult
    {
        public IEnumerable<RoleClaimsResult> Roles { get; set; }
    }
}
