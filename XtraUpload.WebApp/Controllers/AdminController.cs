using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using XtraUpload.Administration.Service.Common;
using XtraUpload.Authentication.Service.Common;
using XtraUpload.Domain;
using XtraUpload.WebApp.Common;
using XtraUpload.Setting.Service.Common;
using XtraUpload.FileManager.Service.Common;
using XtraUpload.Email.Service.Common;
using MediatR;

namespace XtraUpload.WebApp.Controllers
{
    [Authorize(Policy = "Admin")]
    public class AdminController : BaseController
    {
        readonly IMapper _mapper;
        readonly IMediator _mediatr;
        readonly IAdministrationService _administration;

        public AdminController(IAdministrationService administration, IMediator mediatr, IMapper mapper)
        {
            _mapper = mapper;
            _mediatr = mediatr;
            _administration = administration;
        }

        [HttpGet("overview")]
        public async Task<IActionResult> OverView([FromQuery]DateRangeViewModel range)
        {
            AdminOverViewResult Result = await _mediatr.Send(new GetAdminOverViewQuery(range.Start, range.End));
            
            return HandleResult(Result);
        }

        [HttpGet("uploadstats")]
        public async Task<IActionResult> UploadStats([FromQuery]DateRangeViewModel range)
        {
            AdminOverViewResult Result = await _mediatr.Send(new GetUploadStatsQuery(range.Start, range.End));

            return HandleResult(Result, Result.FilesCount);
        }

        [HttpGet("userstats")]
        public async Task<IActionResult> UserStats([FromQuery]DateRangeViewModel range)
        {
            var Result = await _mediatr.Send(new GetUserStatsQuery(range.Start, range.End));

            return HandleResult(Result, Result.UsersCount);
        }

        [HttpGet("filetypesstats")]
        public async Task<IActionResult> FileTypesStats([FromQuery]DateRangeViewModel range)
        {
            var Result = await _mediatr.Send(new GetFileTypeStatsQuery(range.Start, range.End));

            return HandleResult(Result, Result.FileTypesCount);
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetUsers([FromQuery]PageSearchModel model)
        {
            PagingResult<UserExtended> Result = await _mediatr.Send(new GetUsersQuery(model));

            return HandleResult(Result);
        }
        [HttpPatch("user")]
        public async Task<IActionResult> UpdateUser(EditUserCommand command)
        {
            EditUserResult result = await _mediatr.Send(command);

            return HandleResult(result, result.User);
        }

        [HttpDelete("users")]
        public async Task<IActionResult> DeleteUsers(IEnumerable<string> ids)
        {
            OperationResult result = await _mediatr.Send(new DeleteUsersCommand(ids));

            return HandleResult(result);
        }

        [HttpGet("files")]
        public async Task<IActionResult> GetFiles([FromQuery]PageSearchModel pageSearch)
        {
            PagingResult<FileItemExtended> Result = await _mediatr.Send(new GetFilesQuery(pageSearch));

            return HandleResult(Result);
        }

        [HttpGet("fileextensions")]
        public async Task<IActionResult> GetFileExtensions()
        {
            FileExtensionsResult Result = await _administration.GetFileExtensions();

            return HandleResult(Result, Result.FileExtensions);
        }

        [HttpGet("searchusers")]
        public async Task<IActionResult> SearchUsers([FromQuery]SearchUserViewModel model)
        {
            SearchUserResult Result = await _administration.SearchUsers(model.Name);

            return HandleResult(Result, new { users = _mapper.Map<IEnumerable<SearchUserDto>>(Result.Users) });
        }

        [HttpDelete("files")]
        public async Task<IActionResult> DeleteFiles(IEnumerable<string> ids)
        {
            DeleteFilesResult result = await _mediatr.Send(new DeleteFilesCommand(ids));

            return HandleResult(result, result.Files);
        }

        [HttpPost("extension")]
        public async Task<IActionResult> AddExtension(AddExtensionViewModel model)
        {
            FileExtensionResult result = await _administration.AddExtension(model.Name);

            return HandleResult(result, result.FileExtension);
        }

        [HttpPatch("extension")]
        public async Task<IActionResult> UpdateExtension(EditExtensionViewModel ext)
        {
            FileExtensionResult result = await _mediatr.Send(new UpdateExtensionCommand(ext.Id, ext.NewExt));

            return HandleResult(result);
        }

        [HttpDelete("extension/{id:regex(^[[0-9]]*$)}")]
        public async Task<IActionResult> DeleteExtension(int id)
        {
            OperationResult result = await _mediatr.Send(new DeleteExtensionCommand(id));

            return HandleResult(result);
        }

        [HttpGet("groups")]
        public async Task<IActionResult> GetUsersRole()
        {
            RolesResult result = await _administration.GetUsersRole();

            return HandleResult(result, _mapper.Map<IEnumerable<RoleClaimsResultDto>>(result.Roles));
        }

        [HttpPost("groups")]
        public async Task<IActionResult> AddRoleClaims(AddRoleClaimsCommand cmd)
        {
            RoleClaimsResult result = await _mediatr.Send(cmd);

            return HandleResult(result, _mapper.Map<RoleClaimsResultDto>(result));
        }

        [HttpPatch("groups")]
        public async Task<IActionResult> UpdateRoleClaims(UpdateRoleClaimsCommand cmd)
        {
            RoleClaimsResult result = await _mediatr.Send(cmd);

            return HandleResult(result, _mapper.Map<RoleClaimsResultDto>(result));
        }

        [HttpDelete("groups/{roleId:regex(^[[a-zA-Z0-9]]*$)}")]
        public async Task<IActionResult> DeleteRoleClaims(string roleId)
        {
            OperationResult result = await _mediatr.Send(new DeleteRoleClaimsCommand(roleId));

            return HandleResult(result);
        }
        [HttpGet("appsettings")]
        public async Task<IActionResult> GetAppSettings()
        {
            ReadAppSettingResult result = await _mediatr.Send(new GetAppSettingsQuery());

            return HandleResult(result, _mapper.Map<ReadAppSettingResultDto>(result));
        }

        [HttpPatch("jwtOptions")]
        public async Task<IActionResult> UpdateJwtOptions(JwtIssuerOptions model)
        {
            OperationResult result = await _mediatr.Send(new UpdateConfigSectionCommand(model));

            return HandleResult(result);
        }

        [HttpPatch("uploadOptions")]
        public async Task<IActionResult> UpdateUploadOptions(UploadOptions model)
        {
            OperationResult result = await _mediatr.Send(new UpdateConfigSectionCommand(model));

            return HandleResult(result);
        }
        [HttpPatch("emailOptions")]
        public async Task<IActionResult> UpdateEmailOptions(EmailSettings model)
        {
            OperationResult result = await _mediatr.Send(new UpdateConfigSectionCommand(model));

            return HandleResult(result);
        }
        [HttpPatch("hardwareOptions")]
        public async Task<IActionResult> UpdateHardwareOpts(HardwareCheckOptions model)
        {
            OperationResult result = await _mediatr.Send(new UpdateConfigSectionCommand(model));

            return HandleResult(result);
        }
        [HttpPatch("appSettings")]
        public async Task<IActionResult> UpdateAppSettings(WebAppSettings model)
        {
            OperationResult result = await _mediatr.Send(new UpdateConfigSectionCommand(model));

            return HandleResult(result);
        }
        [HttpPatch("socialAuthSettings")]
        public async Task<IActionResult> UpdateSocialAuthSettings(SocialAuthSettings model)
        {
            OperationResult result = await _mediatr.Send(new UpdateConfigSectionCommand(model));

            return HandleResult(result);
        }
        [HttpGet("pages")]
        public async Task<IActionResult> GetPages()
        {
            var result = await _administration.GetPages();

            return HandleResult(result, result.Pages);
        }
        [HttpPost("page")]
        public async Task<IActionResult> AddPage(Page page)
        {
            PageResult result = await _administration.AddPage(page);

            return HandleResult(result, result.Page);
        }
        [HttpPatch("page")]
        public async Task<IActionResult> UpdatePage(Page page)
        {
            PageResult result = await _administration.UpdatePage(page);

            return HandleResult(result, result.Page);
        }
        [HttpDelete("page/{id:regex(^[[a-zA-Z0-9]]*$)}")]
        public async Task<IActionResult> DeletePage(string id)
        {
            OperationResult result = await _administration.DeletePage(id);

            return HandleResult(result);
        }
    }
}
