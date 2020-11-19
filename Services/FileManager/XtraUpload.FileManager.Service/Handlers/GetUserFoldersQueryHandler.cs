using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using XtraUpload.Database.Data.Common;
using XtraUpload.Domain;
using XtraUpload.FileManager.Service.Common;

namespace XtraUpload.FileManager.Service
{
    /// <summary>
    /// Get all user folders
    /// </summary>
    public class GetUserFoldersQueryHandler : IRequestHandler<GetUserFoldersQuery, GetFoldersResult>
    {
        #region Fields
        readonly IUnitOfWork _unitOfWork;
        readonly ClaimsPrincipal _caller;
        #endregion

        #region Constructor
        public GetUserFoldersQueryHandler(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _caller = httpContextAccessor.HttpContext.User;
        }
        #endregion

        #region Handler
        public async Task<GetFoldersResult> Handle(GetUserFoldersQuery request, CancellationToken cancellationToken)
        {
            GetFoldersResult Result = new GetFoldersResult();

            string userId = _caller.GetUserId();
            Result.Folders = await _unitOfWork.Folders.FindAsync(s => s.UserId == userId);

            return Result;
        }
        #endregion
    }
}
