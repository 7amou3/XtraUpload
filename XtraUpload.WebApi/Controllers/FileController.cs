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
                        dest.WaitTime = Result.WaitTime;
                        dest.UserLoggedIn = Response.HttpContext.User.Identity.IsAuthenticated;
                    });
                });
                return Ok(filedto);
            }

            return HandleResult(Result);
        }

        [HttpDelete("deleteitems")]
        public async Task<IActionResult> DeleteItems(DeleteItemsViewModel items)
        {
            DeleteItemsResult result = await _mediator.Send(new DeleteItemsCommand(items.SelectedFolders, items.SelectedFiles));

            return HandleResult(result, _mapper.Map<DeleteItemsResultDto>(result));
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