using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;
using XtraUpload.Domain;
using XtraUpload.Domain.Infra;

namespace XtraUpload.ServerApp.Filters
{
    /// <summary>
    /// Apply a post filter on controller's method operation result
    /// </summary>
    //public class ResultFilter : IActionFilter
    //{
    //    public void OnActionExecuting(ActionExecutingContext filterContext)
    //    {

    //    }

    //    public void OnActionExecuted(ActionExecutedContext filterContext)
    //    {
    //        if (filterContext.Result is OkObjectResult okResult)
    //        {
    //            if (okResult.Value is OperationResult result && result.State != OperationState.Success)
    //            {
    //                var errorRequest = new ContentResult();
    //                string content = Helpers.JsonSerialize(result);
    //                errorRequest.Content = content;
    //                errorRequest.ContentType = "application/json";

    //                if (result.ErrorContent.ErrorType == ErrorOrigin.Client)
    //                {
    //                    filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
    //                }
    //                else
    //                {
    //                    filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
    //                }

    //                filterContext.Result = errorRequest;
    //            }
    //        }
    //    }
    //}
}
