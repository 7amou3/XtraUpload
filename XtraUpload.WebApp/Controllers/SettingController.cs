using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using XtraUpload.Administration.Service.Common;
using XtraUpload.Domain;
using XtraUpload.WebApp.Common;
using XtraUpload.Setting.Service.Common;
using XtraUpload.Authentication.Service.Common;
using MediatR;

namespace XtraUpload.WebApp.Controllers
{
    [Authorize(Policy = "User")]
    public class SettingController : BaseController
    {
        readonly IMapper _mapper;
        readonly IMediator _mediatr;
        readonly WebAppSettings _webappSettings;
        readonly SocialAuthSettings _socialSettings;
        readonly ISettingService _settingService;

        public SettingController(ISettingService settingService, IOptionsMonitor<SocialAuthSettings> socialSettings,
            IOptionsMonitor<WebAppSettings> webappSettings, IMapper mapper, IMediator mediatr)
        {
            _mapper = mapper;
            _settingService = settingService;
            _webappSettings = webappSettings.CurrentValue;
            _socialSettings = socialSettings.CurrentValue;
            _mediatr = mediatr;
        }

        [HttpGet("uploadsetting")]
        public async Task<IActionResult> UploadSetting()
        {
            UploadSettingResult Result = await _mediatr.Send(new GetUploadSettingQuery());

            return HandleResult(Result);
        }

        [HttpGet("accountoverview")]
        public async Task<IActionResult> AccountOverview()
        {
            AccountOverviewResult Result = await _mediatr.Send(new GetAccountOverviewQuery());

            return HandleResult(Result, _mapper.Map<AccountOverviewDto>(Result));
        }

        [HttpPatch("password")]
        public async Task<IActionResult> UpdatePassword(UpdatePassword model)
        {
            UpdatePasswordResult result = await _settingService.UpdatePassword(model);

            return HandleResult(result);
        }

        [HttpPatch("theme")]
        public async Task<IActionResult> UpdateTheme(UpdateThemeCommand cmd)
        {
            OperationResult result = await _mediatr.Send(cmd);

            return HandleResult(result);
        }

        [HttpGet("confirmemail")]
        public async Task<IActionResult> ConfirmEmail()
        {
            OperationResult result = await _mediatr.Send(new RequestConfirmationEmailCommand(Request.Host.Host));

            return HandleResult(result);
        }

        [HttpPut("confirmemail")]
        public async Task<IActionResult> ConfirmEmail(ConfirmEmailViewModel model)
        {
            OperationResult result = await _mediatr.Send(new ConfirmEmailCommand(model.EmailToken));

            return HandleResult(result);
        }

        [AllowAnonymous]
        [HttpGet("page/{name:regex(^[[a-zA-Z0-9_]]*$)}")]
        public async Task<IActionResult> GetPage(string name)
        {
            PageResult result = await _settingService.GetPage(name);

            return HandleResult(result, result.Page);
        }

        [AllowAnonymous]
        [HttpGet("webappconfig")]
        public IActionResult GetWebAppConfig()
        {
            return Ok(_webappSettings);
        }

        [AllowAnonymous]
        [HttpGet("socialauthconfig")]
        public IActionResult GetSocialAuthConfig()
        {
            return Ok(_socialSettings);
        }
    }
}
