using System.Collections.Generic;
using XtraUpload.Domain;

namespace XtraUpload.Authentication.Service.Common
{
    public class RoleClaimsResult : OperationResult
    {
        public Role Role { get; set; }
        public IEnumerable<RoleClaim> Claims { get; set; }
    }
    public class RoleClaims
    {
        public Role Role { get; set; }
        public IEnumerable<RoleClaim> Claims { get; set; }
    }
}
