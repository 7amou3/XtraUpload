using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XtraUpload.Domain;
using XtraUpload.FileManager.Service.Common;

namespace XtraUpload.WebApi.Controllers
{
    [Authorize(Policy = "User")]
    public class FileController : BaseController
    {
        readonly IMapper _mapper;
        readonly IMediator _mediator;

        public FileController(IMediator mediator, IMapper mapper)
        {
            _mapper = mapper;
            _mediator = mediator;
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
            RequestDownloadResult Result = await _mediator.Send(new RequestDownloadQuery(fileid));
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
            DeleteFileResult Result = await _mediator.Send(new DeleteFileCommand(fileid));
           
            return HandleResult(Result, _mapper.Map<FileItemHeaderDto>(Result.File));
        }
        [HttpDelete("deleteitems")]
        public async Task<IActionResult> DeleteItems(DeleteItemsViewModel items)
        {
            DeleteItemsResult result = await _mediator.Send(new DeleteItemsCommand(items.SelectedFolders, items.SelectedFiles));

            return HandleResult(result, _mapper.Map<DeleteItemsResultDto>(result));
        }

        [AllowAnonymous]
        [HttpGet("avatar/{userid:regex(^[[a-zA-Z0-9./-]]*$)}/{timespan?}")]
        public async Task<IActionResult> GetAvatar(string userid, string timespan = null)
        {
            AvatarUrlResult Result = await _mediator.Send(new GetAvatarQuery(userid));

            if (Result.State != OperationState.Success)
            {
                if (Result.ErrorContent.ErrorType == ErrorOrigin.Client)
                {
                    return BadRequest(Result);
                }
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }

            // Do not close the stream, MVC will handle it
            var stream = System.IO.File.OpenRead(Result.Url);

            return new FileStreamResult(stream, "image/png");
        }

        [HttpGet("avatarurl")]
        public async Task<IActionResult> GetAvatarUrl()
        {
            AvatarUrlResult Result = await _mediator.Send(new GetUserAvatarQuery());

            return HandleResult(Result);
        }

        [HttpPatch("fileavailability")]
        public async Task<IActionResult> FileAvailability(FileAvailabilityViewModel fileAvailability)
        {
            FileAvailabilityResult Result = await _mediator.Send(new UpdateFileAvailabilityCommand(fileAvailability.Fileid, fileAvailability.Available));
            
            return HandleResult(Result, _mapper.Map<FileItemDto>(Result.File));
        }

        [HttpPatch("rename")]
        public async Task<IActionResult> Rename(RenameFileViewModel file)
        {
            RenameFileResult Result = await _mediator.Send(new UpdateFileNameCommand(file.FileId, file.NewName));
            
            return HandleResult(Result, _mapper.Map<FileItemHeaderDto>(Result.File));
        }

        [AllowAnonymous]
        [HttpGet("templink/{fileid:regex(^[[a-zA-Z0-9]]*$)}")]
        public async Task<IActionResult> Downloadlink(string fileid)
        {
            TempLinkResult Result = await _mediator.Send(new GenerateTempLinkCommand(fileid));

            return HandleResult(Result, new { downloadurl = Result.StorageServerAddress + "/api/file/download/" + Result.FileDownload.Id });
        }

        [HttpPut("moveitems")]
        public async Task<IActionResult> MoveItems(MoveItemsViewModel items)
        {
            MoveItemsResult Result = await _mediator.Send(new MoveItemsCommand(items.DestFolderId, items.SelectedFiles, items.SelectedFolders));

            return HandleResult(Result);
        }

    }
}