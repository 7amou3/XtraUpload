using System.Collections.Generic;

namespace XtraUpload.WebApp
{
    internal class RoleClaimsResultDto
    {
        public RoleDto Role { get; set; }
        public IEnumerable<RoleClaimDto> Claims { get; set; }
    }
}
