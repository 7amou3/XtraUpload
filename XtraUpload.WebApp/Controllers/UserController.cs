using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using XtraUpload.Authentication.Service.Common;
using XtraUpload.Domain;
using XtraUpload.WebApp.Common;

namespace XtraUpload.WebApp.Controllers
{
    public class UserController : BaseController
    {
        readonly IMapper _mapper;
        readonly IAuthenticationService _authService;

        public UserController(IAuthenticationService authService, IMapper mapper)
        {
            _mapper = mapper;
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> CreateNewAccount(RegistrationViewModel model)
        {
            CreateUserAccountResult result = await _authService.CreateUserAccount(model);

            return HandleResult(result, _mapper.Map<UserDto>(result.NewUser));
        }

        [HttpPost("login")]
        public async Task<IActionResult> StandardAuth(CredentialsViewModel credentials)
        {
            XuIdentityResult result = await _authService.StandardAuth(credentials);

            return IdentityCheck(result);
        }

        [HttpPost("socialauth")]
        public async Task<IActionResult> SocialAuth(SocialMediaLoginViewModel model)
        {
            XuIdentityResult result = await _authService.SocialMediaAuth(model);

            return IdentityCheck(result);
        }

        // POST: api/lostpassword
        [HttpPost("lostpassword")]
        public async Task<IActionResult> LostPassword(LostPasswordViewModel model)
        {
            OperationResult result = await _authService.LostPassword(model.Email, Request.Host.Host);

            return HandleResult(result);
        }

        [HttpGet("pwdrecoveryinfo/{recoveryId:regex(^[[a-zA-Z0-9]]*$)}")]
        public async Task<IActionResult> PasswordRecoveryInfo(string recoveryId)
        {
            OperationResult result = await _authService.CheckRecoveryInfo(recoveryId);

            return HandleResult(result);
        }

        [HttpPut("recoverPassword")]
        public async Task<IActionResult> RecoverPassword(RecoverPasswordViewModel model)
        {
            OperationResult result = await _authService.RecoverPassword(model);

            return HandleResult(result);
        }
        
        private IActionResult IdentityCheck(XuIdentityResult result)
        {
            if (result.State != OperationState.Success)
            {
                return HandleResult(result);
            }

            UserDto response = _mapper.Map<UserDto>(result.User, opts =>
            {
                opts.AfterMap((src, dest) =>
                { 
                    ((UserDto)dest).JwtToken = result.JwtToken;
                    ((UserDto)dest).Role = result.Role.RoleClaims.Any(s => s.ClaimType == XtraUploadClaims.AdminAreaAccess.ToString()) ? "Admin" : "User";
                });
            });
            return Ok(response);
        }
    }
}