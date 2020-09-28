using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using XtraUpload.Domain;
using XtraUpload.StorageManager.Common;

namespace XtraUpload.StorageServer.Controllers
{
    public class FileController : BaseController
    {
        readonly IMediator _mediator;

        public FileController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("smallthumb/{fileid:regex(^[[a-zA-Z0-9]]*$)}")]
        public async Task<IActionResult> GetSmallThumb(string fileid)
        {
            AvatarUrlResult Result = await _mediator.Send(new GetThumbnailQuery(ThumbnailSize.Small, fileid));

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

            return File(stream, "image/png");
        }

        [HttpGet("mediumthumb/{fileid:regex(^[[a-zA-Z0-9]]*$)}")]
        public async Task<IActionResult> GetMediumThumb(string fileid)
        {
            AvatarUrlResult Result = await _mediator.Send(new GetThumbnailQuery(ThumbnailSize.Medium, fileid));

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
    }
}
