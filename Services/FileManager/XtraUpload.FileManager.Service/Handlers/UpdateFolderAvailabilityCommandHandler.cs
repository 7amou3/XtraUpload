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
    /// Update online folder availability
    /// </summary>
    public class UpdateFolderAvailabilityCommandHandler : IRequestHandler<UpdateFolderAvailabilityCommand, FolderAvailabilityResult>
    {
        #region Fields
        readonly IUnitOfWork _unitOfWork;
        readonly ClaimsPrincipal _caller;
        #endregion

        #region Constructor
        public UpdateFolderAvailabilityCommandHandler(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _caller = httpContextAccessor.HttpContext.User;
        }
        #endregion

        #region Handler
        public async Task<FolderAvailabilityResult> Handle(UpdateFolderAvailabilityCommand request, CancellationToken cancellationToken)
        {
            FolderAvailabilityResult Result = new FolderAvailabilityResult();

            string userId = _caller.Claims.FirstOrDefault(c => c.Type == "id")?.Value;

            var folder = await _unitOfWork.Folders.FirstOrDefaultAsync(s => s.Id == request.FolderId && s.UserId == userId);
            // Check if folder exist
            if (folder == null)
            {
                Result.ErrorContent = new ErrorContent("No folder with the provided id was found", ErrorOrigin.Client);
                return Result;
            }

            // Prepare data
            folder.Status = request.IsOnline ? ItemStatus.Visible : ItemStatus.Hidden;
            folder.LastModified = DateTime.UtcNow;

            // Try to save in db
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
