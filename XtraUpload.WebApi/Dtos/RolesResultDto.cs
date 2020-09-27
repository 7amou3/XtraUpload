using System.Collections.Generic;

namespace XtraUpload.WebApi
{
    internal class RolesResultDto
    {
        public IEnumerable<RoleClaimsResultDto> Roles { get; set; }
    }
}
