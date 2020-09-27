using System.Collections.Generic;

namespace XtraUpload.WebApi
{
    internal class RoleClaimsResultDto
    {
        public RoleDto Role { get; set; }
        public IEnumerable<RoleClaimDto> Claims { get; set; }
    }
}
