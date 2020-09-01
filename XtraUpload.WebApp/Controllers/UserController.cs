using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
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
        readonly IMediator _mediator;

        public UserController(IAuthenticationService authService, IMediator mediator, IMapper mapper)
        {
            _mapper = mapper;
            _mediator = mediator;
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> CreateNewAccount(CreateUserViewModel cmd)
        {
            var user = new User()
            {
                Email = cmd.Email,
                UserName = cmd.UserName,
                Password = cmd.Password
            };
            CreateUserResult result = await _mediator.Send(new CreateUserCommand(user));

            return HandleResult(result, _mapper.Map<UserDto>(result.User));
        }

        [HttpPost("login")]
        public async Task<IActionResult> StandardAuth(StandardLoginQuery credentials)
        {
            XuIdentityResult result = await _mediator.Send(credentials);

            return IdentityCheck(result);
        }

        [HttpPost("socialauth")]
        public async Task<IActionResult> SocialAuth(SocialMediaLoginQuery model)
        {
            XuIdentityResult result = await _mediator.Send(model);

            return IdentityCheck(result);
        }

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