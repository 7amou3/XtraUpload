using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using XtraUpload.Database.Data.Common;
using XtraUpload.Domain;
using XtraUpload.Domain.Infra;
using XtraUpload.FileManager.Service.Common;

namespace XtraUpload.FileManager.Service
{
    /// <summary>
    /// Get user's avatar url
    /// </summary>
    public class GetUserAvatarQueryHandler : IRequestHandler<GetUserAvatarQuery, AvatarUrlResult>
    {
        #region Fields
        readonly IUnitOfWork _unitOfWork;
        readonly ClaimsPrincipal _caller;
        #endregion

        #region Constructor
        public GetUserAvatarQueryHandler(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _caller = httpContextAccessor.HttpContext.User;
        }
        #endregion

        #region Handler
        public async Task<AvatarUrlResult> Handle(GetUserAvatarQuery request, CancellationToken cancellationToken)
        {
            AvatarUrlResult result = new AvatarUrlResult();
            string userId = _caller.GetUserId();
            var user = await _unitOfWork.Users.FirstOrDefaultAsync(s => s.Id == userId);
            if (user == null)
            {
                result.ErrorContent = new ErrorContent("No user with the provided id was found", ErrorOrigin.Client);
                return result;
            }
            result.Url = user.Avatar;
            return result;
        }
        #endregion
    }
}
