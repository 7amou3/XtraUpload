using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using System;
using System.Net;
using XtraUpload.Domain;

namespace XtraUpload.WebApi
{
    /// <summary>
    /// Disable HTTP verbs on demo version
    /// </summary>
    public class DemoFilter : Attribute, IResourceFilter
    {
        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            IConfiguration config = context.HttpContext.RequestServices.GetService(typeof(IConfiguration)) as IConfiguration;
            if (config != null && bool.TryParse(config.GetSection("DisableAdminActions").Value, out bool disableAdminActions))
            {
                // demo mode is disabled
                if (!disableAdminActions)
                {
                    return;
                }
            }

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
