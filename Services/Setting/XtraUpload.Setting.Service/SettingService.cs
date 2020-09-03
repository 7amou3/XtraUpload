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

       
        #endregion

    }
}
