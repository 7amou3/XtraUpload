using MediatR;
using AutoMapper;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XtraUpload.Domain;
using XtraUpload.Setting.Service.Common;
using XtraUpload.Administration.Service.Common;
using System.Collections.Generic;

namespace XtraUpload.WebApi.Controllers
{
    [Authorize(Policy = "User")]
    public class SettingController : BaseController
    {
        readonly IMapper _mapper;
        readonly IMediator _mediatr;

        public SettingController(IMediator mediatr, IMapper mapper)
        {
            _mapper = mapper;
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
        public async Task<IActionResult> UpdatePassword(UpdatePasswordViewModel model)
        {
            UpdatePasswordResult result = await _mediatr.Send(new UpdatePasswordCommand(model.OldPassword, model.NewPassword));

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
            OperationResult result = await _mediatr.Send(new RequestConfirmationEmailCommand(Request.HttpContext.Connection.RemoteIpAddress.ToString()));

            return HandleResult(result);
        }

        [HttpPut("confirmemail")]
        public async Task<IActionResult> ConfirmEmail(ConfirmEmailViewModel model)
        {
            OperationResult result = await _mediatr.Send(new ConfirmEmailCommand(model.EmailToken));

            return HandleResult(result);
        }

        [AllowAnonymous]
        [HttpGet("page/{url:regex(^[[a-zA-Z0-9_]]*$)}")]
        public async Task<IActionResult> GetPage(string url)
        {
            PageResult result = await _mediatr.Send(new GetPageQuery(url));

            return HandleResult(result, result.Page);
        }

        [AllowAnonymous]
        [HttpGet("appinitializerconfig")]
        public async Task<IActionResult> GetAppInitializerConfig()
        {
            AppInitializerConfigResult result = await _mediatr.Send(new GetAppInitializerConfigQuery());
            return HandleResult(result, new {
                result.AppInfo,
                result.Version,
                PagesHeader = _mapper.Map<IEnumerable<PageHeaderDto>>(result.Pages)
            });
        }

        [AllowAnonymous]
        [HttpGet("socialauthconfig")]
        public async Task<IActionResult> GetSocialAuthConfig()
        {
            ReadAppSettingResult result = await _mediatr.Send(new GetAppSettingsQuery());

            return Ok(result.SocialAuthSettings);
        }

        [AllowAnonymous]
        [HttpGet("languages")]
        public async Task<IActionResult> GetLanguages()
        {
            LanguagesResult result = await _mediatr.Send(new GetLanguagesQuery());

            return HandleResult(result, _mapper.Map<IEnumerable<LanguageDto>>(result.Languages));
        }

        [HttpPatch("language")]
        public async Task<IActionResult> UpdateLanguage(UpdateLanguageCommand command)
        {
            OperationResult result = await _mediatr.Send(command);

            return HandleResult(result);
        }
    }
}
