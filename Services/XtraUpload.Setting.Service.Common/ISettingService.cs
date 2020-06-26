using System;
using System.Threading.Tasks;
using XtraUpload.Administration.Service.Common;
using XtraUpload.Domain;
using XtraUpload.WebApp.Common;

namespace XtraUpload.Setting.Service.Common
{
    public interface ISettingService
    {
        /// <summary>
        /// User account overview
        /// </summary>
        Task<AccountOverviewResult> AccountOverview();

        /// <summary>
        /// Upload Settings
        /// </summary>
        Task<UploadSettingResult> UploadSetting();

        /// <summary>
        /// Updates a user password
        /// </summary>
        Task<UpdatePasswordResult> UpdatePassword(UpdatePasswordViewModel model);

        /// <summary>
        /// Update the user theme
        /// </summary>
        Task<OperationResult> UpdateTheme(Theme theme);

        /// <summary>
        /// Request a confirmation email
        /// </summary>
        Task<OperationResult> RequestConfirmationEmail(string clienIp);

        /// <summary>
        /// Confirm email based on the provided token
        /// </summary>
        Task<OperationResult> ConfirmEmail(string emailToken);

        /// <summary>
        /// Get a page by name
        /// </summary>
        Task<PageResult> GetPage(string pageName);
    }
}
