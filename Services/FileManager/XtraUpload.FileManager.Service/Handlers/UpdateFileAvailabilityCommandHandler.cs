using Askmethat.Aspnet.JsonLocalizer.Localizer;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using XtraUpload.Database.Data.Common;
using XtraUpload.Domain;
using XtraUpload.FileManager.Service.Common;

namespace XtraUpload.FileManager.Service
{
    /// <summary>
    /// Updates the online availability state of the given file
    /// </summary>
    public class UpdateFileAvailabilityCommandHandler : IRequestHandler<UpdateFileAvailabilityCommand, FileAvailabilityResult>
    {
        #region Fields
        readonly IUnitOfWork _unitOfWork;
        readonly ClaimsPrincipal _caller;
        readonly IJsonStringLocalizer _localizer;
        #endregion

        #region Constructor
        public UpdateFileAvailabilityCommandHandler(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, IJsonStringLocalizer localizer)
        {
            _unitOfWork = unitOfWork;
            _localizer = localizer;
            _caller = httpContextAccessor.HttpContext.User;
        }
        #endregion

        #region Handler
        public async Task<FileAvailabilityResult> Handle(UpdateFileAvailabilityCommand request, CancellationToken cancellationToken)
        {
            FileAvailabilityResult Result = new FileAvailabilityResult();

            string userId = _caller.GetUserId();
            FileItem file = await _unitOfWork.Files.FirstOrDefaultAsync(s => s.Id == request.FileId && s.UserId == userId);

            // Check file exist
            if (file == null)
            {
                Result.ErrorContent = new ErrorContent(_localizer["No file with the provided id was found"], ErrorOrigin.Client);
                return Result;
            }

            // Prepare data
            file.Status = request.IsOnline ? ItemStatus.Visible : ItemStatus.Hidden;
            file.LastModified = DateTime.UtcNow;

            // Try to save in db
            Result = await _unitOfWork.CompleteAsync(Result);
            if (Result.State == OperationState.Success)
            {
                Result.File = file;
            }

            return Result;
        }
        #endregion
    }
}
