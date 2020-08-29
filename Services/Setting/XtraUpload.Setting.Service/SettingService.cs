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
        readonly UploadOptions _uploadOpt;
        readonly IEmailService _emailService; 
        readonly ILogger<SettingService> _logger;

        #region Constructor
        public SettingService(IEmailService emailService, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, IOptions<UploadOptions> uploadOpt,
            ILogger<SettingService> logger)
        {
            _logger = logger;
            _emailService = emailService;
            _uploadOpt = uploadOpt.Value;
            _unitOfWork = unitOfWork;
            _caller = httpContextAccessor.HttpContext.User;
        }
        #endregion

        #region ISettingService members

        /// <summary>
        /// Get the upload setting for the connected user
        /// </summary>
        public async Task<UploadSettingResult> UploadSetting()
        {
            string userId = _caller.GetUserId();

            UploadSettingResult Result = new UploadSettingResult();
            try
            {
                var extensions = await _unitOfWork.FileExtensions.GetAll();
                Result.UsedSpace = await _unitOfWork.Files.SumAsync(s => s.UserId == userId, s => s.Size);
                Result.StorageSpace = double.Parse(_caller.Claims.Single(c => c.Type == "StorageSpace").Value);
                Result.ConcurrentUpload = int.Parse(_caller.Claims.Single(c => c.Type == "ConcurrentUpload").Value);
                Result.MaxFileSize = int.Parse(_caller.Claims.Single(c => c.Type == "MaxFileSize").Value);
                Result.ChunkSize = _uploadOpt.ChunkSize * 1024 * 1024;
                Result.FileExtensions = string.Join(", ", extensions.Select(s => s.Name));
            }
            catch (Exception _ex)
            {
                Result.ErrorContent = new ErrorContent(_ex.Message, ErrorOrigin.Server);
                #region Trace
                _logger.LogError(_ex.Message);
                #endregion
            }

            return Result;
        }

        /// <summary>
        /// User account overview
        /// </summary>
        public async Task<AccountOverviewResult> AccountOverview()
        {
            string userId = _caller.GetUserId();

            AccountOverviewResult Result = new AccountOverviewResult
            {
                // Read the download setting from jwt token
                DownloadSetting = new DownloadSettingResult()
                {
                    DownloadSpeed = int.Parse(_caller.Claims.Single(c => c.Type == "DownloadSpeed").Value) * 1024,
                    DownloadTTW = int.Parse(_caller.Claims.Single(c => c.Type == "DownloadTTW").Value),
                    FileExpiration = int.Parse(_caller.Claims.Single(c => c.Type == "FileExpiration").Value),
                    TimeToWait = int.Parse(_caller.Claims.Single(c => c.Type == "WaitTime").Value),
                },

                // Get the upload settings
                UploadSetting = await UploadSetting()
            };
            if (Result.UploadSetting.State != OperationState.Success)
            {
                Result = OperationResult.CopyResult<AccountOverviewResult>(Result.UploadSetting);
                return Result;
            }

            // Get the files stats
            Result.FilesStats = new FilesStatsResult()
            {
                TotalDownloads = await _unitOfWork.Files.SumAsync(s => s.UserId == userId, s => s.DownloadCount),
                TotalFiles = await _unitOfWork.Files.CountAsync(s => s.UserId == userId)
            };

            // Get user info
            Result.User = await _unitOfWork.Users.FirstOrDefaultAsync(s => s.Id == userId);
            
            return Result;
        }

        /// <summary>
        /// Update the user theme
        /// </summary>
        public async Task<OperationResult> UpdateTheme(Theme theme)
        {
            string userId = _caller.GetUserId();
            OperationResult result = new OperationResult();
            User user = await _unitOfWork.Users.FirstOrDefaultAsync(s => s.Id == userId);
            // Check user exist
            if (user == null)
            {
                result.ErrorContent = new ErrorContent("No user found with the provided email.", ErrorOrigin.Client);
                return result;
            }
            // Update
            user.Theme = theme;
            user.LastModified = DateTime.Now;
            // Save to db
            try
            {
                await _unitOfWork.CompleteAsync();
            }
            catch (Exception _ex)
            {
                result.ErrorContent = new ErrorContent("Unknown error occured, please try again", ErrorOrigin.Server);
                _logger.LogError(_ex.Message);
            }

            return result;
        }

        /// <summary>
        /// Updates a user password
        /// </summary>
        public async Task<UpdatePasswordResult> UpdatePassword(UpdatePasswordViewModel model)
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
            try
            {
                await _unitOfWork.CompleteAsync();
            }
            catch (Exception _ex)
            {
                Result.ErrorContent = new ErrorContent("Unknown error occured, please try again", ErrorOrigin.Server);
                _logger.LogError(_ex.Message);
            }

            return Result;
        }

        /// <summary>
        /// Request a confirmation email
        /// </summary>
        public async Task<OperationResult> RequestConfirmationEmail(string clientIp)
        {
            string userId = _caller.GetUserId();
            OperationResult Result = new OperationResult();
            User user = await _unitOfWork.Users.FirstOrDefaultAsync(s => s.Id == userId);
            // Check user exist
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
            // Check email confirmation status
            if (user.EmailConfirmed)
            {
                Result.ErrorContent = new ErrorContent("The email has been already confirmed.", ErrorOrigin.Client);
                return Result;
            }
            // Create and store a mail confirmation token
            ConfirmationKey token = new ConfirmationKey()
            {
                Id = Helpers.GenerateUniqueId(),
                GenerateAt = DateTime.Now,
                Status = RequestStatus.InProgress,
                UserId = userId,
                IpAdress = clientIp
            };
            _unitOfWork.ConfirmationKeys.Add(token);
            try
            {
                await _unitOfWork.CompleteAsync();
            }
            catch (Exception _ex)
            {
                Result.ErrorContent = new ErrorContent("Unknown error occured, please try again", ErrorOrigin.Server);
                _logger.LogError(_ex.Message);
            }

            // Send Pass recovery email 
            _emailService.SendConfirmEmail(token, user);

            return Result;
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

            try
            {
                await _unitOfWork.CompleteAsync();
            }
            catch (Exception _ex)
            {
                Result.ErrorContent = new ErrorContent("Unknown error occured, please try again", ErrorOrigin.Server);
                _logger.LogError(_ex.Message);
            }

            return Result;
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
