using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System;
using System.Net;
using System.Threading.Tasks;
using tusdotnet.Models;
using tusdotnet.Models.Concatenation;
using tusdotnet.Models.Configuration;
using tusdotnet.Models.Expiration;
using tusdotnet.Stores;
using Microsoft.Extensions.DependencyInjection;
using XtraUpload.Domain.Infra;
using Microsoft.Extensions.Options;
using XtraUpload.WebApp.Common;
using XtraUpload.FileManager.Service.Common;

namespace XtraUpload.FileManager.Service
{
    /// <summary>
    /// Base class for file uploads
    /// </summary>
    public abstract class BaseFileUpload
    {
        #region Fields
        protected readonly IServiceProvider _serviceProvider;
        protected readonly ILogger<FileUploadService> _logger;
        private readonly string _urlPath;
        #endregion

        #region Constructor
        public BaseFileUpload(IServiceProvider serviceProvider, string urlPath)
        {
            _urlPath = urlPath;
            _serviceProvider = serviceProvider;
            _logger = serviceProvider.GetService<ILoggerFactory>().CreateLogger<FileUploadService>();
        }
        #endregion

        /// <summary>
        /// Factory called for every http tus upload request
        /// </summary>
        public DefaultTusConfiguration CreateTusConfiguration()
        {
            UploadOptions uploadOpts = _serviceProvider.GetService<IOptionsMonitor<UploadOptions>>().CurrentValue;

            return new DefaultTusConfiguration
            {
                UrlPath = _urlPath,
                MaxAllowedUploadSizeInBytes = int.MaxValue,
                MaxAllowedUploadSizeInBytesLong = int.MaxValue,
                Store = new TusDiskStore(uploadOpts.UploadPath),
                Expiration = new AbsoluteExpiration(TimeSpan.FromMinutes(uploadOpts.Expiration)),
                Events = new Events
                {
                    OnAuthorizeAsync = OnAuthorize,
                    OnBeforeCreateAsync = OnBeforeCreate,
                    OnFileCompleteAsync = OnUploadCompleted
                }
            };
        }

        /// <summary>
        /// Handle the pre upload required metadata 
        /// </summary>
        private Task OnBeforeCreate(BeforeCreateContext ctx)
        {
            // Partial files are not complete so we do not need to validate the metadata in our example.
            if (ctx.FileConcatenation is FileConcatPartial)
            {
                return Task.CompletedTask;
            }
            if (!ctx.Metadata.ContainsKey("name"))
            {
                ctx.FailRequest("name metadata must be specified.");
            }
            if (!ctx.Metadata.ContainsKey("contentType"))
            {
                ctx.FailRequest("contentType metadata must be specified.");
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Handle the upload authorization
        /// </summary>
        private Task OnAuthorize(AuthorizeContext ctx)
        {
            string userId = ctx.HttpContext.User.GetUserId();
            if (!ctx.HttpContext.User.Identity.IsAuthenticated || userId == null)
            {
                ctx.HttpContext.Response.Headers.Add("WWW-Authenticate", new StringValues("Basic realm=XtraUpload"));
                ctx.FailRequest(HttpStatusCode.Unauthorized);
                return Task.CompletedTask;
            }

            switch (ctx.Intent)
            {
                case IntentType.CreateFile:
                    break;
                case IntentType.ConcatenateFiles:
                    break;
                case IntentType.WriteFile:
                    break;
                case IntentType.DeleteFile:
                    break;
                case IntentType.GetFileInfo:
                    break;
                case IntentType.GetOptions:
                    break;
                default:
                    break;
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Handle the upload completion
        /// </summary>
        protected abstract Task OnUploadCompleted(FileCompleteContext ctx);
    }
}
