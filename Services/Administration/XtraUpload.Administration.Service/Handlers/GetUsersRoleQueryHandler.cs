using MediatR;
using System.Threading;
using System.Threading.Tasks;
using XtraUpload.Administration.Service.Common;
using XtraUpload.Database.Data.Common;

namespace XtraUpload.Administration.Service
{
    /// <summary>
    /// Get the users role
    /// </summary>
    public class GetUsersRoleQueryHandler : IRequestHandler<GetUsersRoleQuery, RolesResult>
    {
        readonly IUnitOfWork _unitOfWork;
        public GetUsersRoleQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<RolesResult> Handle(GetUsersRoleQuery request, CancellationToken cancellationToken)
        {
            RolesResult Result = new RolesResult()
            {
                Roles = await _unitOfWork.Users.GetUsersRoleClaims()
            };
            return Result;
        }
    }
}
