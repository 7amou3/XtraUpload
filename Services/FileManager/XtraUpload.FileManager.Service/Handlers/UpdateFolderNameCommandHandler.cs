using Askmethat.Aspnet.JsonLocalizer.Localizer;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using XtraUpload.Database.Data.Common;
using XtraUpload.Domain;
using XtraUpload.FileManager.Service.Common;

namespace XtraUpload.FileManager.Service
{
    /// <summary>
    /// Update folder name
    /// </summary>
    public class UpdateFolderNameCommandHandler : IRequestHandler<UpdateFolderNameCommand, RenameFolderResult>
    {
        #region Fields
        readonly IUnitOfWork _unitOfWork;
        readonly ClaimsPrincipal _caller;
        readonly IJsonStringLocalizer _localizer;
        #endregion

        #region Constructor
        public UpdateFolderNameCommandHandler(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, IJsonStringLocalizer localizer)
        {
            _localizer = localizer;
            _unitOfWork = unitOfWork;
            _caller = httpContextAccessor.HttpContext.User;
        }
        #endregion

        #region Handler
        public async Task<RenameFolderResult> Handle(UpdateFolderNameCommand request, CancellationToken cancellationToken)
        {
            RenameFolderResult Result = new RenameFolderResult();
            string userId = _caller.Claims.Single(c => c.Type == "id")?.Value;
            FolderItem folder = await _unitOfWork.Folders.FirstOrDefaultAsync(s => s.Id == request.FolderId && s.UserId == userId);
            // Check if folder exist
            if (folder == null)
            {
                Result.ErrorContent = new ErrorContent(_localizer["No folder with the provided id was found"], ErrorOrigin.Client);
                return Result;
            }

            // Prepare data
            folder.Name = request.NewName;
            folder.LastModified = DateTime.UtcNow;

            // Try to save to db
            Result = await _unitOfWork.CompleteAsync(Result);
            if (Result.State == OperationState.Success)
            {
                Result.Folder = folder;
            }

            return Result;
        }
        #endregion
    }
}
