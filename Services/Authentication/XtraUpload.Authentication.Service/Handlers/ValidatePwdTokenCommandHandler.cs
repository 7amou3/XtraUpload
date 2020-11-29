using Askmethat.Aspnet.JsonLocalizer.Localizer;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XtraUpload.Authentication.Service.Common;
using XtraUpload.Database.Data.Common;
using XtraUpload.Domain;

namespace XtraUpload.Authentication.Service
{
    /// <summary>
    /// Update the password based on the provided token
    /// </summary>
    public class ValidatePwdTokenCommandHandler : IRequestHandler<ValidatePwdTokenCommand, OperationResult>
    {
        #region Fields
        readonly IUnitOfWork _unitOfWork;
        readonly IJsonStringLocalizer _localizer;
        #endregion

        #region Constructor
        public ValidatePwdTokenCommandHandler(IUnitOfWork unitOfWork, IJsonStringLocalizer localizer)
        {
            _localizer = localizer;
            _unitOfWork = unitOfWork;
        }
        #endregion

        #region Handler
        public async Task<OperationResult> Handle(ValidatePwdTokenCommand request, CancellationToken cancellationToken)
        {
            OperationResult Result = new OperationResult();
            ConfirmationKey recoveryInfo = await _unitOfWork.ConfirmationKeys.FirstOrDefaultAsync(s => s.Id == request.RecoveryKey);

            // Check recovery info exists
            if (recoveryInfo == null)
            {
                Result.ErrorContent = new ErrorContent(_localizer["The provided token does not exist"], ErrorOrigin.Client);
                return Result;
            }

            if (recoveryInfo.Status != RequestStatus.InProgress)
            {
                Result.ErrorContent = new ErrorContent(_localizer["The provided token has already been used or expired"], ErrorOrigin.Client);
                return Result;
            }

            // Update the user's password
            User user = await _unitOfWork.Users.FirstOrDefaultAsync(s => s.Id == recoveryInfo.UserId);
            if (user == null)
            {
                Result.ErrorContent = new ErrorContent(_localizer["No user found with the provided id."], ErrorOrigin.Client);
                return Result;
            }
            user.Password = Helpers.HashPassword(request.NewPassword);
            user.LastModified = DateTime.UtcNow;

            // Update the recovery status
            recoveryInfo.Status = RequestStatus.Completed;

            // Save changes
            return await _unitOfWork.CompleteAsync(Result);
        }
        #endregion
    }
}
