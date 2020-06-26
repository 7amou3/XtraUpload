using System.Collections.Generic;

namespace XtraUpload.WebApp
{
    internal class RolesResultDto
    {
        public IEnumerable<RoleClaimsResultDto> Roles { get; set; }
    }
}
