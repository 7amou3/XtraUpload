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
    /// Get a public folder content
    /// </summary>
    class GetPublicFolderQueryHandler : IRequestHandler<GetPublicFolderQuery, GetFolderContentResult>
    {
        #region Fields
        readonly IUnitOfWork _unitOfWork;
        readonly ClaimsPrincipal _caller;
        #endregion

        #region Constructor
        public GetPublicFolderQueryHandler(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _caller = httpContextAccessor.HttpContext.User;
        }
        #endregion

        #region Handler
        public async Task<GetFolderContentResult> Handle(GetPublicFolderQuery request, CancellationToken cancellationToken)
        {
            GetFolderContentResult Result = new GetFolderContentResult();
            string userId = _caller.GetUserId();
            // Display child folder if requested
            string folderId = request.ChildFolderId ?? request.MainFolderId;
            FolderItem folder = await _unitOfWork.Folders.FirstOrDefaultAsync(f => f.Id == folderId);
            // Check if folder exist
            if (folder == null)
            {
                Result.ErrorContent = new ErrorContent("No folder with the provided id was found", ErrorOrigin.Client);
                return Result;
            }
            // If anonymous user, check if folder is public
            if (userId != folder.UserId && folder.IsAvailableOnline == false)
            {
                Result.ErrorContent = new ErrorContent("This folder is not available for public downloads", ErrorOrigin.Client);
                return Result;
            }

            // Get folders
            Result.Folders = await _unitOfWork.Folders.FindAsync(s => s.Parentid == folderId);
            // get Files, the root folder is represented by a null value in TFile table
            Result.Files = await _unitOfWork.Files.FindAsync(s => s.FolderId == folderId);

            return Result;
        }
        #endregion
    }
}
