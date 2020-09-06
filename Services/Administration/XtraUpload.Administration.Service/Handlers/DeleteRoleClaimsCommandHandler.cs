using MediatR;
using System.Threading;
using System.Threading.Tasks;
using XtraUpload.Administration.Service.Common;
using XtraUpload.Database.Data.Common;
using XtraUpload.Domain;

namespace XtraUpload.Administration.Service
{
    /// <summary>
    /// Delete a role claim
    /// </summary>
    public class DeleteRoleClaimsCommandHandler : IRequestHandler<DeleteRoleClaimsCommand, OperationResult>
    {
        readonly IUnitOfWork _unitOfWork;

        public DeleteRoleClaimsCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<OperationResult> Handle(DeleteRoleClaimsCommand request, CancellationToken cancellationToken)
        {
            OperationResult result = new OperationResult();
            Role role = await _unitOfWork.Roles.FirstOrDefaultAsync(s => s.Id == request.RoleId);
            if (role == null)
            {
                result.ErrorContent = new ErrorContent("No role found with the provided id.", ErrorOrigin.Client);
                return result;
            }
            // Default role can not be deleted
            if (role.IsDefault)
            {
                result.ErrorContent = new ErrorContent("Default role can not be deleted.", ErrorOrigin.Client);
                return result;
            }

            _unitOfWork.Roles.Remove(role);

            // Save to db
            return await _unitOfWork.CompleteAsync(result);
        }
    }
}
