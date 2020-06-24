using System.Collections.Generic;

namespace XtraUpload.ServerApp
{
    internal class RoleClaimsResultDto
    {
        public RoleDto Role { get; set; }
        public IEnumerable<RoleClaimDto> Claims { get; set; }
    }
}
