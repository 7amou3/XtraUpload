using System;
using Microsoft.Extensions.Logging;
using XtraUpload.Database.Data.Common;
using System.Threading.Tasks;
using XtraUpload.Authentication.Service.Common;
using XtraUpload.Domain;
using System.Security.Claims;
using System.Linq;
using Microsoft.Extensions.Options;
using XtraUpload.ServerApp.Common;
using XtraUpload.Domain.Infra;
using XtraUpload.Email.Service.Common;
using System.Collections.Generic;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace XtraUpload.Authentication.Service
{
    public class AuthenticationService: IAuthenticationService
    {
        #region Fields
        readonly IUnitOfWork _unitOfWork;
        readonly IJwtFactory _jwtFactory;
        readonly JwtIssuerOptions _jwtOpts;
        readonly IEmailService _emailService;
        readonly ILogger<AuthenticationService> _logger;
        #endregion

        #region Constructor
        public AuthenticationService(IUnitOfWork unitOfWork, IEmailService emailService, IOptionsSnapshot<JwtIssuerOptions> jwtOpts, IJwtFactory jwtFactory, ILogger<AuthenticationService> logger)
        {
            _logger = logger;
            _jwtOpts = jwtOpts.Value;
            _jwtFactory = jwtFactory;
            _emailService = emailService;
            _unitOfWork = unitOfWork;
        }
        #endregion

        #region IAuthenticationService members

        /// <summary>
        /// Create a new user account
        /// </summary>
        public async Task<CreateUserAccountResult> CreateUserAccount(RegistrationViewModel model)
        {
            CreateUserAccountResult Result = new CreateUserAccountResult();

            // Check if it's not a duplicated user
            User user = await _unitOfWork.Users.FirstOrDefaultAsync(s => s.Email == model.Email);
            if (user != null)
            {
                Result.ErrorContent = new ErrorContent("A user with this email already exist", ErrorOrigin.Client);
                return Result;
            }

            // Add the new user to the memory collection
            user = new User()
            {
                UserName = model.UserName,
                Email = model.Email,
                Password = Helpers.HashPassword(model.Password),
                CreatedAt = DateTime.Now,
                LastModified = DateTime.Now,
                Provider = AuthProvider.STANDARD,
                RoleId = "2", // is the basic user rol see OnModelCreating of ApplicationDbContext
                Theme = Theme.Light
            };
            _unitOfWork.Users.Add(user);

            // try to save to db
            try
            {
                await _unitOfWork.CompleteAsync();
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
                Result.ErrorContent = new ErrorContent("Unknown error occured, please try again", ErrorOrigin.Server);
                return Result;
            }

            await PostRegistration(user);

            Result.NewUser = user;
            return Result;
        }

        /// <summary>
        /// Check user identity
        /// </summary>
        public async Task<XuIdentityResult> StandardAuth(CredentialsViewModel credentials)
        {
            XuIdentityResult Result = new XuIdentityResult();

            User user = await _unitOfWork.Users.FirstOrDefaultAsync(u => u.Email == credentials.Email);
            // Check the user exist
            if (user == null)
            {
                Result.ErrorContent = new ErrorContent("No user found with the provided email.", ErrorOrigin.Client);
                return Result;
            }
            // Check user is not suspended
            if (user.AccountSuspended)
            {
                Result.ErrorContent = new ErrorContent("Your Account Has Been Suspended.", ErrorOrigin.Client);
                return Result;
            }
            // Check the password
            if (!Helpers.CheckPassword(credentials.Password, user.Password))
            {
                Result.ErrorContent = new ErrorContent("Email or Password does not match", ErrorOrigin.Client);
                return Result;
            }

            // Get user claims
            Result = await GetUserClaims(user, credentials.RememberMe);

            #region Trace
            if (Result.State != OperationState.Success)
            { 
                _logger.LogError(Result.ErrorContent.ToString());
            }
            #endregion

            return Result;
        }

        /// <summary>
        /// Manages the connection or account creation with social media (fb, google...)
        /// </summary>
        public async Task<XuIdentityResult> SocialMediaAuth(SocialMediaLoginViewModel model)
        {
            XuIdentityResult Result = new XuIdentityResult();
            // Ready to create the local user account (if necessary) and jwt
            User userInfo = await _unitOfWork.Users.FirstOrDefaultAsync(u => u.Email == model.Email);

            // Create the new user
            if (userInfo == null)
            {
                userInfo = new User()
                {
                    Email = model.Email,
                    EmailConfirmed = false,
                    UserName = model.Name,
                    Password = Helpers.GenerateUniqueId(),
                    Avatar = model.PhotoUrl,
                    SocialMediaId = model.Id,
                    CreatedAt = DateTime.Now,
                    LastModified = DateTime.Now,
                    Provider = (AuthProvider)Enum.Parse(typeof(AuthProvider), model.Provider),
                    RoleId = "2", // is the basic user rol see OnModelCreating of ApplicationDbContext,
                    Theme = Theme.Light
                };
                _unitOfWork.Users.Add(userInfo);

                // Try to save to db
                try
                {
                    await _unitOfWork.CompleteAsync();
                }
                catch (Exception _ex)
                {
                    Result.ErrorContent = new ErrorContent("Unknown error occured, please try again", ErrorOrigin.Server);
                    _logger.LogError(_ex.Message);
                }

                await PostRegistration(userInfo);
            }

            // Check user is not suspended
            if (userInfo.AccountSuspended)
            {
                Result.ErrorContent = new ErrorContent("Your Account Has Been Suspended.", ErrorOrigin.Client);
                return Result;
            }

            Result = await GetUserClaims(userInfo, false);
            return Result;
        }

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
            try
            {
                await _unitOfWork.CompleteAsync();
                // if success, send an email to the user
                _emailService.SendPassRecovery(token, user);
            }
            catch(Exception _ex)
            {
                Result.ErrorContent = new ErrorContent("Unknown error occured, please try again", ErrorOrigin.Server);
                _logger.LogError(_ex.Message);
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
        #endregion

        /// <summary>
        /// Get user claims
        /// </summary>
        private async Task<XuIdentityResult> GetUserClaims(User user, bool rememberMe = false)
        {
            XuIdentityResult Result = new XuIdentityResult();
            RoleClaimsResult claimsResult = await _unitOfWork.Users.GetUserRoleClaims(user);
            
            Result.User = user;
            Result.Role = claimsResult.Role;
            Result.ClaimsIdentity = _jwtFactory.GenerateClaimsIdentity(Result.User, claimsResult.Claims);
            Result.JwtToken = await GenerateJwt(Result.ClaimsIdentity, Result.User, rememberMe);
            
            return Result;
        }

        /// <summary>
        /// Generate a jwt token
        /// </summary>
        private async Task<JwtToken> GenerateJwt(ClaimsIdentity identity, User user, bool rememberUser)
        {
            if (rememberUser)
            {
                // Rember me = 30 day token validity
                _jwtOpts.ValidFor = 30;
            }
            return new JwtToken
            {
                Token = await _jwtFactory.GenerateEncodedToken(user.UserName, identity),
                Expires_in = (int)_jwtOpts.ValidFor * 24 * 60 * 60  
            };
        }

        /// <summary>
        /// Should be called when a new user has been added 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private async Task PostRegistration(User user)
        {
            // Create default folders tree for this user
            _unitOfWork.Folders.AddRange(GenerateDefaultFolders(user.Id));

            // Store a confirmation email token
            _unitOfWork.ConfirmationKeys.Add(new ConfirmationKey()
            {
                GenerateAt = DateTime.Now,
                Id = Helpers.GenerateUniqueId(),
                Status = RequestStatus.InProgress,
                UserId = user.Id
            });
            // todo: send a welcome email with email confirmation request

            try
            {
                await _unitOfWork.CompleteAsync();
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
            }
        }

        /// <summary>
        /// Generate a default folder structure for the newly created user
        /// </summary>
        private IEnumerable<FolderItem> GenerateDefaultFolders(string userId)
        {

            List<FolderItem> folders = new List<FolderItem>()
            {
                new FolderItem()
                {
                    Id = Helpers.GenerateUniqueId(),
                    Name = "Documents",
                    Parentid = "root",
                    UserId = userId,
                    CreatedAt = DateTime.Now,
                    LastModified = DateTime.Now
                },
                new FolderItem()
                {
                    Id = Helpers.GenerateUniqueId(),
                    Name = "Images",
                    Parentid = "root",
                    UserId = userId,
                    CreatedAt = DateTime.Now,
                    LastModified = DateTime.Now
                },
                new FolderItem()
                {
                    Id = Helpers.GenerateUniqueId(),
                    Name = "Videos",
                    Parentid = "root",
                    UserId = userId,
                    CreatedAt = DateTime.Now,
                    LastModified = DateTime.Now
                }
            };

            return folders;
        }
    }
}
