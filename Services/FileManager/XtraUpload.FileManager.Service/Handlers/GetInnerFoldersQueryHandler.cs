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
    /// Get folder(s) relative to it's parent id
    /// </summary>
    public class GetInnerFoldersQueryHandler : IRequestHandler<GetInnerFoldersQuery, GetFoldersResult>
    {
        #region Fields
        readonly IMediator _mediator;
        readonly IUnitOfWork _unitOfWork;
        readonly ClaimsPrincipal _caller;
        #endregion

        #region Constructor
        public GetInnerFoldersQueryHandler(IUnitOfWork unitOfWork, IMediator mediator, IHttpContextAccessor httpContextAccessor)
        {
            _mediator = mediator;
            _unitOfWork = unitOfWork;
            _caller = httpContextAccessor.HttpContext.User;
        }
        #endregion

        #region Handler
        public async Task<GetFoldersResult> Handle(GetInnerFoldersQuery request, CancellationToken cancellationToken)
        {
            GetFoldersResult Result = new GetFoldersResult();
            string userId = _caller.GetUserId();

            FolderItem folder = await _unitOfWork.Folders.FirstOrDefaultAsync(s => s.Id == request.ParentId);
            if (folder == null)
            {
                Result.ErrorContent = new ErrorContent("No folder with the provided id was found", ErrorOrigin.Client);
                return Result;
            }
            // If anonymous user, check if folder is public
            if (userId != folder.UserId && folder.Status != ItemStatus.Visible)
            {
                Result.ErrorContent = new ErrorContent("This folder is not available for public downloads", ErrorOrigin.Client);
                return Result;
            }

            Result.Folders = await _mediator.Send(new GetFoldersRecursivelyQuery(folder));

            return Result;
        }
        #endregion
    }
}
