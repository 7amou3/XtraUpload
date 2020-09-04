using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using XtraUpload.Domain;
using XtraUpload.Domain.Infra;
using XtraUpload.FileManager.Service.Common;
using XtraUpload.WebApp.Common;

namespace XtraUpload.WebApp.Controllers
{

    [Authorize(Policy = "User")]
    public class FileController : BaseController
    {
        readonly IMapper _mapper;
        readonly IMediator _mediator;
        readonly UploadOptions _uploadOpts;
        readonly IFileManagerService _filemanagerService;
        readonly IFileDownloadService _fileDownloadService;

        public FileController(IFileManagerService filemanagerService, IFileDownloadService fileDownloadService, IOptionsMonitor<UploadOptions> uploadOpts, IMediator mediator, IMapper mapper)
        {
            _mapper = mapper;
            _mediator = mediator;
            _uploadOpts = uploadOpts.CurrentValue;
            _filemanagerService = filemanagerService;
            _fileDownloadService = fileDownloadService;
        }

        [HttpGet("{tusid:regex(^[[a-zA-Z0-9]]*$)}")]
        public async Task<IActionResult> Get(string tusid)
        {
            GetFileResult Result = await _mediator.Send(new GetFileByTusIdQuery(tusid));

            return HandleResult(Result, Result.File);
        }

        [AllowAnonymous]
        [HttpGet("requestdownload/{fileid:regex(^[[a-zA-Z0-9]]*$)}")]
        public async Task<IActionResult> RequestDownload(string fileid)
        {
            RequestDownloadResult Result = await _fileDownloadService.RequestDownload(fileid);
            if (Result.State == OperationState.Success)
            {
                var filedto = _mapper.Map<FileItemDto>(Result.File, opts =>
                {
                    opts.AfterMap((src, dest) =>
                    {
                        ((FileItemDto)dest).WaitTime = Result.WaitTime;
                        ((FileItemDto)dest).UserLoggedIn = Response.HttpContext.User.Identity.IsAuthenticated;
                    });
                });
                return Ok(filedto);
            }

            return HandleResult(Result);
        }

        [HttpDelete("{fileid:regex(^[[a-zA-Z0-9]]*$)}")]
        public async Task<IActionResult> Delete(string fileid)
        {
            DeleteFileResult Result = await _filemanagerService.DeleteFile(fileid);
           
            return HandleResult(Result, _mapper.Map<FileItemHeaderDto>(Result.File));
        }
        [HttpDelete("deleteitems")]
        public async Task<IActionResult> DeleteItems(DeleteItemsViewModel items)
        {
            DeleteItemsResult result = await _filemanagerService.DeleteItems(items);

            return HandleResult(result, _mapper.Map<DeleteItemsResultDto>(result));
        }

        [AllowAnonymous]
        [HttpGet("smallthumb/{fileid:regex(^[[a-zA-Z0-9]]*$)}")]
        public async Task<IActionResult> GetSmallThumb(string fileid)
        {
            GetFileResult Result = await _mediator.Send(new GetFileByIdQuery(fileid));

            if (Result.State != OperationState.Success)
            {
                if (Result.ErrorContent.ErrorType == ErrorOrigin.Client)
                {
                    return BadRequest(Result);
                }
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }

            string filePath = Path.Combine(_uploadOpts.UploadPath, Result.File.UserId.ToString(), Result.File.Id, Result.File.Id + ".smallthumb.png");
            
            if (!System.IO.File.Exists(filePath))
            {
                return BadRequest("The file has not been found");
            }

            using var img = System.IO.File.OpenRead(filePath);
            return PhysicalFile(filePath, "image/png");
        }

        [AllowAnonymous]
        [HttpGet("mediumthumb/{fileid:regex(^[[a-zA-Z0-9]]*$)}")]
        public async Task<IActionResult> GetMediumThumb(string fileid)
        {
            GetFileResult Result = await _mediator.Send(new GetFileByIdQuery(fileid));

            if (Result.State != OperationState.Success)
            {
                if (Result.ErrorContent.ErrorType == ErrorOrigin.Client)
                {
                    return BadRequest(Result);
                }
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }

            string filePath = Path.Combine(_uploadOpts.UploadPath, Result.File.UserId.ToString(), Result.File.Id, Result.File.Id + ".mediumthumb.png");

            if (!System.IO.File.Exists(filePath))
            {
                // the image has no medium thumb because it's small
                filePath = Path.Combine(_uploadOpts.UploadPath, Result.File.UserId.ToString(), Result.File.Id, Result.File.Id + ".smallthumb.png");
                if (!System.IO.File.Exists(filePath))
                {
                    return BadRequest("The file has not been found");
                }
            }

            using var img = System.IO.File.OpenRead(filePath);
            return PhysicalFile(filePath, "image/png");
        }

        [HttpPatch("fileavailability")]
        public async Task<IActionResult> FileAvailability(FileAvailabilityViewModel fileAvailability)
        {
            FileAvailabilityResult Result = await _filemanagerService.UpdateFileAvailability(fileAvailability.Fileid, fileAvailability.Available);
            
            return HandleResult(Result, _mapper.Map<FileItemDto>(Result.File));
        }

        [HttpPatch("rename")]
        public async Task<IActionResult> Rename(RenameFileViewModel file)
        {
            RenameFileResult Result = await _filemanagerService.UpdateFileName(file.FileId, file.NewName);
            
            return HandleResult(Result, _mapper.Map<FileItemHeaderDto>(Result.File));
        }

        [AllowAnonymous]
        [HttpGet("templink/{fileid:regex(^[[a-zA-Z0-9]]*$)}")]
        public async Task<IActionResult> Downloadlink(string fileid)
        {
            TempLinkResult Result = await _fileDownloadService.TempLink(fileid);

            return HandleResult(Result, new { downloadurl = BaseUrl + "/api/file/download/" + Result.FileDownload.Id });
        }

        [AllowAnonymous]
        [HttpGet("download/{downloadid:regex(^[[a-zA-Z0-9]]*$)}")]
        public async Task Download(string downloadid)
        {
            await _fileDownloadService.StartDownload(downloadid);
        }

        [HttpPut("moveitems")]
        public async Task<IActionResult> MoveItems(MoveItemsViewModel items)
        {
            MoveItemsResult Result = await _filemanagerService.MoveItems(items);

            return HandleResult(Result);
        }

        [HttpGet("avatarurl")]
        public async Task<IActionResult> GetAvatarUrl()
        {
            AvatarResult Result = await _filemanagerService.GetUserAvatar();

            return HandleResult(Result);
        }

        [AllowAnonymous]
        [HttpGet("avatar/{userid:regex(^[[a-zA-Z0-9./-]]*$)}/{timespan?}")]
        public IActionResult GetAvatar(string userid, string timespan = null)
        {
            string filePath = Path.Combine(_uploadOpts.UploadPath, userid, "avatar", "avatar.png");

            if (!System.IO.File.Exists(filePath))
            {
                return BadRequest("The file has not been found");
            }

            using var img = System.IO.File.OpenRead(filePath);
            return PhysicalFile(filePath, "image/png");
        }

    }
}