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
using Microsoft.Extensions.Options;
using XtraUpload.StorageManager.Common;
using XtraUpload.Protos;
using XtraUpload.Domain;

namespace XtraUpload.StorageManager.Service
{
    /// <summary>
    /// Base class for file uploads
    /// </summary>
    public abstract class BaseFileUpload
    {
        #region Fields
        private readonly string _urlPath;
        protected readonly IOptionsMonitor<UploadOptions> _uploadOpts;
        protected readonly ILogger<FileUploadService> _logger;
        protected readonly gFileStorage.gFileStorageClient _storageClient;
        #endregion

        #region Constructor
        public BaseFileUpload(
            gFileStorage.gFileStorageClient storageClient,
            IOptionsMonitor<UploadOptions> uploadOpts,
            ILogger<FileUploadService> logger,
            string urlPath)
        {
            _urlPath = urlPath;
            _storageClient = storageClient;
            _uploadOpts = uploadOpts;
            _logger = logger;
        }
        #endregion

        /// <summary>
        /// Factory called for every http tus upload request
        /// </summary>
        public DefaultTusConfiguration CreateTusConfiguration()
        {
            return new DefaultTusConfiguration
            {
                UrlPath = _urlPath,
                MaxAllowedUploadSizeInBytes = int.MaxValue,
                MaxAllowedUploadSizeInBytesLong = int.MaxValue,
                Store = new TusDiskStore(_uploadOpts.CurrentValue.UploadPath),
                Expiration = new AbsoluteExpiration(TimeSpan.FromMinutes(_uploadOpts.CurrentValue.Expiration)),
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
        private async Task OnAuthorize(AuthorizeContext ctx)
        {
            var authResponse = await _storageClient.IsAuthorizedAsync(new gIsAuthorizedRequest());
            if (authResponse == null)
            {
                ctx.FailRequest(HttpStatusCode.RequestTimeout);
                return;
            }
            if (authResponse.Status.Status == Protos.RequestStatus.Failed)
            {
                ctx.HttpContext.Response.Headers.Add("WWW-Authenticate", new StringValues("Basic realm=XtraUpload"));
                ctx.FailRequest(HttpStatusCode.Unauthorized);
                return;
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

            return;
        }

        /// <summary>
        /// Handle the upload completion
        /// </summary>
        protected abstract Task OnUploadCompleted(FileCompleteContext ctx);
    }
}
