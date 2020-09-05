using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using XtraUpload.Database.Data.Common;
using XtraUpload.Domain;
using XtraUpload.Setting.Service.Common;

namespace XtraUpload.Setting.Service
{
    /// <summary>
    /// Confirm email based on the provided token
    /// </summary>
    public class ConfirmEmailCommandHandler : IRequestHandler<ConfirmEmailCommand, OperationResult>
    {
        readonly IUnitOfWork _unitOfWork;
        
        public ConfirmEmailCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        
        public async Task<OperationResult> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
        {
            OperationResult Result = new OperationResult();

            ConfirmationKeyResult confirmationKey = await _unitOfWork.Users.GetConfirmationKeyInfo(request.EmailToken);

            if (confirmationKey == null)
            {
                Result.ErrorContent = new ErrorContent("No email found with the provided token.", ErrorOrigin.Client);
                return Result;
            }
            if (confirmationKey.Key.Status != RequestStatus.InProgress)
            {
                Result.ErrorContent = new ErrorContent("The provided token has already been used or expired", ErrorOrigin.Client);
                return Result;
            }
            if (confirmationKey.User.EmailConfirmed)
            {
                Result.ErrorContent = new ErrorContent("The email associated with the provided token has been already confirmed.", ErrorOrigin.Client);
                return Result;
            }

            confirmationKey.User.EmailConfirmed = true;
            confirmationKey.User.LastModified = DateTime.Now;

            // Save changes to db
            return await _unitOfWork.CompleteAsync(Result);
        }
    }
}
