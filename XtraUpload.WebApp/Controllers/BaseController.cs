using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using XtraUpload.Domain;
using XtraUpload.WebApp.Filters;

namespace XtraUpload.WebApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ServiceFilter(typeof(ApiExceptionFilter))]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public abstract class BaseController : ControllerBase
    {
        protected Uri BaseUrl
        {
            get
            {
                return new Uri(Request.Scheme + "://" + Request.Host + Request.PathBase);
            }
        }

        protected IActionResult HandleResult<T>(T result) where T: OperationResult
        {
            if (result.State != OperationState.Success)
            {
                if (result.ErrorContent.ErrorType == ErrorOrigin.Client)
                {
                    return BadRequest(result);
                }
                return StatusCode((int)HttpStatusCode.InternalServerError, result);
            }

            return Ok(result);
        }

        protected IActionResult HandleResult<T, R>(T result, R entity) where T : OperationResult
        {
            if (result.State != OperationState.Success)
            {
                if (result.ErrorContent.ErrorType == ErrorOrigin.Client)
                {
                    return BadRequest(result);
                }
                return StatusCode((int)HttpStatusCode.InternalServerError);
            }

            return Ok(entity);
        }

    }
}
