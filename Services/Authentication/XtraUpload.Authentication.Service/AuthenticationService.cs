using System;
using XtraUpload.Database.Data.Common;
using System.Threading.Tasks;
using XtraUpload.Authentication.Service.Common;
using XtraUpload.Domain;
using XtraUpload.WebApp.Common;
using XtraUpload.Domain.Infra;
using XtraUpload.Email.Service.Common;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace XtraUpload.Authentication.Service
{
    public class AuthenticationService: IAuthenticationService
    {
        #region Fields
        readonly IUnitOfWork _unitOfWork;
        readonly IEmailService _emailService;
        #endregion

        #region Constructor
        public AuthenticationService(IUnitOfWork unitOfWork, IEmailService emailService)
        {
            _emailService = emailService;
            _unitOfWork = unitOfWork;
        }
        #endregion

        #region IAuthenticationService members

        

        /// <summary>
        /// Manages the lost password action
        /// </summary>
        public async Task<OperationResult> LostPassword(string email, string clientIp)
        {
            OperationResult Result = new OperationResult();
            User user = await _unitOfWork.Users.FirstOrDefaultAsync(s => s.Email == email);

            // Check the user exist
            if (user == null)
            {
                Result.ErrorContent = new ErrorContent("No user found with the provided email.", ErrorOrigin.Client);
                return Result;
            }
            // Check email service is up
            HealthCheckResult health = await (_emailService as IHealthCheck).CheckHealthAsync(null);
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
                IpAdress = clientIp
            };
            // add the key to current collection
            _unitOfWork.ConfirmationKeys.Add(token);

            // Save to db
            Result = await _unitOfWork.CompleteAsync(Result);
            if (Result.State == OperationState.Success)
            {
                // if success, send an email to the user
                _emailService.SendPassRecovery(token, user);
            }

            return Result;
        }

        /// <summary>
        /// Gets Password recovey info by id
        /// </summary>
        public async Task<OperationResult> CheckRecoveryInfo(string recoeryId)
        {
            OperationResult Result = new OperationResult();
            ConfirmationKey recoveryInfo = await _unitOfWork.ConfirmationKeys.FirstOrDefaultAsync(s => s.Id == recoeryId);

            // Check recovery info exists
            if (recoveryInfo == null)
            {
                Result.ErrorContent = new ErrorContent("The provided token does not exist", ErrorOrigin.Client);
                return Result;
            }

            // Generated link expires after 24h (by a background alien thread)
            if (DateTime.Now > recoveryInfo.GenerateAt.AddDays(1))
            {
                Result.ErrorContent = new ErrorContent("The provided token was expired", ErrorOrigin.Client);
            }
            if (recoveryInfo.Status != RequestStatus.InProgress)
            {
                Result.ErrorContent = new ErrorContent("The provided token has already been used or expired", ErrorOrigin.Client);
            }

            return Result;
        }

        /// <summary>
        /// Update the recoverd password
        /// </summary>
        public async Task<OperationResult> RecoverPassword(RecoverPasswordViewModel model)
        {
            OperationResult Result = new OperationResult();
            ConfirmationKey recoveryInfo = await _unitOfWork.ConfirmationKeys.FirstOrDefaultAsync(s => s.Id == model.RecoveryKey);
            
            // Check recovery info exists
            if (recoveryInfo == null)
            {
                Result.ErrorContent = new ErrorContent("The provided token does not exist", ErrorOrigin.Client);
                return Result;
            }

            if (recoveryInfo.Status != RequestStatus.InProgress)
            {
                Result.ErrorContent = new ErrorContent("The provided token has already been used or expired.", ErrorOrigin.Client);
                return Result;
            }

            // Update the user's password
            User user = await _unitOfWork.Users.FirstOrDefaultAsync(s => s.Id == recoveryInfo.UserId);
            if (user == null)
            {
                Result.ErrorContent = new ErrorContent("No user found with the provided id.", ErrorOrigin.Client);
                return Result;
            }
            user.PasswordHash = Helpers.HashPassword(model.NewPassword);
            user.LastModified = DateTime.Now;

            // Update the recovery status
            recoveryInfo.Status = RequestStatus.Completed;

            // Commit changes
            return await _unitOfWork.CompleteAsync(Result);
        }
        #endregion

       

    }
}
