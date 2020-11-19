using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using XtraUpload.Authentication.Service.Common;
using XtraUpload.Database.Data.Common;
using XtraUpload.Domain;
using XtraUpload.Email.Service.Common;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace XtraUpload.Authentication.Service
{
    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, OperationResult>
    {
        #region Fields
        readonly IMediator _mediatr;
        readonly IUnitOfWork _unitOfWork;
        #endregion

        #region Constructor
        public ResetPasswordCommandHandler(IUnitOfWork unitOfWork, IMediator mediatr)
        {
            _mediatr = mediatr;
            _unitOfWork = unitOfWork;
        }
        #endregion

        #region Handler

        public async Task<OperationResult> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            OperationResult Result = new OperationResult();
            User user = await _unitOfWork.Users.FirstOrDefaultAsync(s => s.Email == request.Email);

            // Check the user exist
            if (user == null)
            {
                Result.ErrorContent = new ErrorContent("No user found with the provided email.", ErrorOrigin.Client);
                return Result;
            }
            // Check email service is up
            HealthCheckResult health = await _mediatr.Send(new EmailHealthCheckQuery());
            if (health.Status != HealthStatus.Healthy)
            {
                Result.ErrorContent = new ErrorContent("Internal email server error, please check again later.", ErrorOrigin.Server);
                return Result;
            }
            // Generate a password reset candidate
            ConfirmationKey token = new ConfirmationKey()
            {
                Id = Helpers.GenerateUniqueId(),
                Status = RequestStatus.InProgress,
                GenerateAt = DateTime.Now,
                UserId = user.Id,
                IpAdress = request.ClientIp
            };
            // add the key to current collection
            await _unitOfWork.ConfirmationKeys.AddAsync(token);

            // Save to db
            Result = await _unitOfWork.CompleteAsync(Result);
            if (Result.State == OperationState.Success)
            {
                // if success, send an email to the user
                await _mediatr.Send(new SendPassRecoveryCommand(user, token));
            }

            return Result;
        }
        #endregion
    }
}
