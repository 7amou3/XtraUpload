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
    /// Update the file name
    /// </summary>
    public class UpdateFileNameCommandHandler : IRequestHandler<UpdateFileNameCommand, RenameFileResult>
    {
        #region Fields
        readonly IUnitOfWork _unitOfWork;
        readonly ClaimsPrincipal _caller;
        readonly IJsonStringLocalizer _localizer;
        #endregion

        #region Constructor
        public UpdateFileNameCommandHandler(IUnitOfWork unitOfWork, IJsonStringLocalizer localizer, IHttpContextAccessor httpContextAccessor)
        {
            _localizer = localizer;
            _unitOfWork = unitOfWork;
            _caller = httpContextAccessor.HttpContext.User;
        }
        #endregion

        #region Handler
        public async Task<RenameFileResult> Handle(UpdateFileNameCommand request, CancellationToken cancellationToken)
        {
            RenameFileResult Result = new RenameFileResult();
            string userId = _caller.GetUserId();
            FileItem file = await _unitOfWork.Files.FirstOrDefaultAsync(s => s.Id == request.FileId && s.UserId == userId);
            // Check if file exist
            if (file == null)
            {
                Result.ErrorContent = new ErrorContent(_localizer["No file with the provided id was found"], ErrorOrigin.Client);
                return Result;
            }

            // Prepare data
            file.Name = request.NewName;
            file.LastModified = DateTime.UtcNow;

            // Try to save to db
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
