using MediatR;
using XtraUpload.Authentication.Service.Common;
using XtraUpload.Domain;

namespace XtraUpload.Administration.Service.Common
{
    /// <summary>
    /// Updates claims of a role
    /// </summary>
    public class UpdateRoleClaimsCommand : IRequest<RoleClaimsResult>
    {
        public Role Role { get; set; }
        public XuClaims Claims { get; set; }
    }
}
