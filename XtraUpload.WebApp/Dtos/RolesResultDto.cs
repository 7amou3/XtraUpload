using System.Collections.Generic;

namespace XtraUpload.ServerApp
{
    internal class RolesResultDto
    {
        public IEnumerable<RoleClaimsResultDto> Roles { get; set; }
    }
}
