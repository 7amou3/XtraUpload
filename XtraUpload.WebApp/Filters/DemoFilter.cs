using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Net;
using XtraUpload.Domain;
using XtraUpload.Domain.Infra;

namespace XtraUpload.WebApp.Filters
{
    /// <summary>
    /// Disable HTTP verbs on demo version
    /// </summary>
    public class DemoFilter : Attribute, IResourceFilter
    {
        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            if (context.HttpContext.Request.Method == "POST"
                || context.HttpContext.Request.Method == "PATCH"
                || context.HttpContext.Request.Method == "DELETE")
            {
                OperationResult result = new OperationResult() { ErrorContent = new ErrorContent("Action disabled in demo version.", ErrorOrigin.Client) };
                string content = Helpers.JsonSerialize(result);
                context.Result = new ContentResult()
                {
                    Content = content,
                    ContentType = "application/json"
                };
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }
        }

        public void OnResourceExecuted(ResourceExecutedContext context)
        {
        }
    }
}