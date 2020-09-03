using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using XtraUpload.Administration.Service.Common;
using XtraUpload.Database.Data.Common;
using XtraUpload.Domain;
using XtraUpload.Domain.Infra;
using XtraUpload.Email.Service.Common;
using XtraUpload.WebApp.Common;
using XtraUpload.Setting.Service.Common;
using XtraUpload.FileManager.Service.Common;

namespace XtraUpload.Setting.Service
{
    public class SettingService: ISettingService
    {
        readonly IUnitOfWork _unitOfWork;
        readonly ClaimsPrincipal _caller;

        #region Constructor
        public SettingService( IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _caller = httpContextAccessor.HttpContext.User;
        }
        #endregion

        #region ISettingService members

        /// <summary>
        /// Updates a user password
        /// </summary>
        public async Task<UpdatePasswordResult> UpdatePassword(UpdatePassword model)
        {
            string userId = _caller.GetUserId();
            UpdatePasswordResult Result = new UpdatePasswordResult();
            User user = await _unitOfWork.Users.FirstOrDefaultAsync(s => s.Id == userId);

            // Check user exist
            if (user == null)
            {
                Result.ErrorContent = new ErrorContent("No user found with the provided email.", ErrorOrigin.Client);
                return Result;
            }

            // Check password match
            if (!Helpers.CheckPassword(model.OldPassword, user.Password))
            {
                Result.ErrorContent = new ErrorContent("Your current password is incorrect.", ErrorOrigin.Client);
                return Result;
            }

            // Update the password in the collection
            user.PasswordHash = Helpers.HashPassword(model.NewPassword);
            user.LastModified = DateTime.Now;

            // Save to db
            return await _unitOfWork.CompleteAsync(Result);
        }

        /// <summary>
        /// Confirm email based on the provided token
        /// </summary>
        /// <returns></returns>
        public async Task<OperationResult> ConfirmEmail(string emailToken)
        {
            OperationResult Result = new OperationResult();

            ConfirmationKeyResult confirmationKey = await _unitOfWork.Users.GetConfirmationKeyInfo(emailToken);

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

        /// <summary>
        /// Get a page by name
        /// </summary>
        public async Task<PageResult> GetPage(string pageName)
        {
            PageResult result = new PageResult();
            Page page = await _unitOfWork.Pages.FirstOrDefaultAsync(s => s.Url.ToLower() == pageName);
            if (page == null)
            {
                result.ErrorContent = new ErrorContent("Page not found.", ErrorOrigin.Client);
                return result;
            }

            result.Page = page;
            return result;
        }
        #endregion

    }
}
