using MediatR;
using XtraUpload.Domain;

namespace XtraUpload.Administration.Service.Common
{
    /// <summary>
    /// Delete a role claim
    /// </summary>
    public class DeleteRoleClaimsCommand : IRequest<OperationResult>
    {
        public DeleteRoleClaimsCommand(string roleId)
        {
            RoleId = roleId;
        }
        public string RoleId { get; set; }
    }
}
