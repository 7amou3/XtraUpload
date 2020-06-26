using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using tusdotnet.Models;
using tusdotnet.Models.Concatenation;
using tusdotnet.Models.Configuration;
using tusdotnet.Models.Expiration;
using tusdotnet.Stores;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using XtraUpload.Database.Data.Common;
using System.IO;
using tusdotnet.Interfaces;
using XtraUpload.Domain;
using XtraUpload.Domain.Infra;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;
using Microsoft.Extensions.Options;
using XtraUpload.WebApp.Common;

namespace XtraUpload.FileManager.Service
{
    /// <summary>
    /// A singleton class used to handle the tusdotnet upload notification events
    /// XtraUpload uses the tus protocol to handle uploading large files efficiently
    /// </summary>
    public class FileUploadService
    {
        #region Fields
        readonly UploadOptions _uploadOpts;
        readonly IServiceProvider _serviceProvider;
        readonly ILogger<FileUploadService> _logger;
        #endregion

        #region Constructor
        public FileUploadService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _uploadOpts = serviceProvider.GetService<IOptionsMonitor<UploadOptions>>().CurrentValue;
            _logger = serviceProvider.GetService<ILoggerFactory>().CreateLogger<FileUploadService>();
        }
        #endregion

        /// <summary>
        /// Called at startup whenever a tus upload request is issued
        /// </summary>
        public DefaultTusConfiguration CreateTusConfiguration()
        {
            return new DefaultTusConfiguration
            {
                UrlPath = "/fileupload",
                MaxAllowedUploadSizeInBytes = int.MaxValue,
                MaxAllowedUploadSizeInBytesLong = int.MaxValue,
                Store = new TusDiskStore(_uploadOpts.UploadPath),
                Expiration = new AbsoluteExpiration(TimeSpan.FromMinutes(_uploadOpts.Expiration)),

                Events = new Events
                {
                    OnAuthorizeAsync = OnAuthorize,
                    OnBeforeCreateAsync = OnBeforeCreate,
                    OnFileCompleteAsync = OnUploadCompleted
                }
            };
        }

        /// <summary>
        /// Handle the upload authorization
        /// </summary>
        private Task OnAuthorize(AuthorizeContext ctx)
        {
            string userId = ctx.HttpContext.User.Claims.FirstOrDefault(s => s.Type == "id")?.Value;
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
        /// Handle the upload completion
        /// </summary>
        private async Task OnUploadCompleted(FileCompleteContext ctx)
        {
            try
            {
                string userId = ctx.HttpContext.User.Claims.First(s => s.Type == "id").Value;
                FileItem file = await PersistMetaData(ctx, userId);
                MoveFilesToFolder(ctx, file);
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
                throw _ex;
            }
        }

        /// <summary>
        /// Persist the uploaded file data to db
        /// </summary>
        private async Task<FileItem> PersistMetaData(FileCompleteContext ctx, string userId)
        {
            ITusFile file = await ((ITusReadableStore)ctx.Store).GetFileAsync(ctx.FileId, ctx.CancellationToken);
            var metadata = await file.GetMetadataAsync(ctx.CancellationToken);

            FileItem fileitem = new FileItem()
            {
                Id = Helpers.GenerateUniqueId(),
                TusId = ctx.FileId,
                UserId = userId,
                Size = new FileInfo(Path.Combine(_uploadOpts.UploadPath, file.Id)).Length,
                Name = metadata["name"].GetString(Encoding.UTF8),
                MimeType = metadata["contentType"].GetString(Encoding.UTF8),
                // file with no folderid is placed in the virtual root folder
                FolderId = metadata["folderId"].GetString(Encoding.UTF8) == "root" 
                                                                                ? null 
                                                                                : metadata["folderId"].GetString(Encoding.UTF8),
                Extension = Helpers.GetFileExtension(metadata["contentType"].GetString(Encoding.UTF8)),
                CreatedAt = DateTime.Now,
                LastModified = DateTime.Now,
                IsAvailableOnline = true
            };

            // Add the uploaded file to db
            using IServiceScope scope = _serviceProvider.CreateScope();
            IUnitOfWork unitOfWork = scope.ServiceProvider.GetService<IUnitOfWork>();
            unitOfWork.Files.Add(fileitem);

            // Try to save to db
            try
            {
                await unitOfWork.CompleteAsync();
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
                throw new Exception("Unhandled exception thrown.");
            }
            
            return fileitem;
        }

        /// <summary>
        /// Moves the uploaded files to the user folder
        /// tus protocol puts the uploaded files into the store, XtraUpload move those files to the user directory
        /// </summary>
        private void MoveFilesToFolder(FileCompleteContext ctx, FileItem fileItem)
        {
            string userFolder = Path.Combine(_uploadOpts.UploadPath, fileItem.UserId);
            string destFolder = Path.Combine(userFolder, fileItem.Id);
            string newFileFullPath = Path.Combine(destFolder, fileItem.Id);
            // Create user root directory
            if (!Directory.Exists(userFolder))
            {
                Directory.CreateDirectory(userFolder);
            }
            // Create a new directory inside the user root dir
            if (!Directory.Exists(destFolder))
            {
                Directory.CreateDirectory(destFolder);
            }

            // move all files to the destination folder
            DirectoryInfo directoryInfo = new DirectoryInfo(_uploadOpts.UploadPath);
            foreach (var file in directoryInfo.GetFiles(ctx.FileId + "*"))
            {
                // Exemple of file names generated by tus are (...69375.metadata, ...69375.uploadlength ...)
                string[] subNames = file.Name.Split('.');
                string subName = subNames.Count() == 2 ? '.' + subNames[1] : string.Empty;
                File.Move(file.FullName, newFileFullPath + subName);
            }

            // Create thumbnails for img file (less than 15mb)
            if (fileItem.MimeType.StartsWith("image") && fileItem.Size < (1024L * 1024L * 15))
            {
                // Todo: move the process of cropping images to a job
                using FileStream smallThumboutStream = new FileStream(newFileFullPath + ".smallthumb.png", FileMode.Create);
                using Image image = Image.Load(File.ReadAllBytes(newFileFullPath), out IImageFormat format);
                if (image.Width >= 800 || image.Height >= 800)
                {
                    int width = 960, height = 640;
                    int aspectRatio = image.Width / image.Height;
                    if (aspectRatio == 0)
                    {
                        height = 960;
                        width = 640;
                    }
                    using FileStream mediumThumboutStream = new FileStream(newFileFullPath + ".mediumthumb.png", FileMode.Create);
                    Image mediumthumbnail = image.Clone(i => i.Resize(width, height).Crop(new Rectangle(0, 0, width, height)));
                    mediumthumbnail.Save(mediumThumboutStream, format);
                }

                Image smallthumbnail = image.Clone(i => i.Resize(128, 128).Crop(new Rectangle(0, 0, 128, 128)));
                smallthumbnail.Save(smallThumboutStream, format);
            }
        }
    }
}
