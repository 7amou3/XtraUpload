using System.Threading.Tasks;
using tusdotnet.Models.Configuration;
using System.Linq;
using System.IO;
using tusdotnet.Interfaces;
using XtraUpload.Domain;
using XtraUpload.Domain.Infra;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Processing;
using Microsoft.Extensions.Options;
using System;
using XtraUpload.StorageManager.Common;
using System.Text;
using Microsoft.Extensions.Logging;
using XtraUpload.Protos;
using Google.Protobuf.WellKnownTypes;

namespace XtraUpload.StorageManager.Service
{
    /// <summary>
    /// A singleton class used to handle the tusdotnet file uploads
    /// XtraUpload uses the tus protocol to handle uploading large files efficiently
    /// </summary>
    public class FileUploadService : BaseFileUpload
    {
        public FileUploadService(
            gFileStorage.gFileStorageClient storageClient,
            IOptionsMonitor<UploadOptions> uploadOpts,
            ILogger<FileUploadService> logger
            ) : base(storageClient, uploadOpts, logger, "/fileupload")
        {
        }

        /// <summary>
        /// Handle the upload completion
        /// </summary>
        protected override async Task OnUploadCompleted(FileCompleteContext ctx)
        {
            try
            {
                gFileItem file = await PersistMetaData(ctx);
                if (file != null)
                {
                    MoveFilesToFolder(ctx, file);

                    // Attach file info to header, because tus send 204 (no response body is allowed)
                    ctx.HttpContext.Response.Headers.Add("upload-data", Helpers.JsonSerialize(file));
                }
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
        private async Task<gFileItem> PersistMetaData(FileCompleteContext ctx)
        {
            ITusFile file = await ((ITusReadableStore)ctx.Store).GetFileAsync(ctx.FileId, ctx.CancellationToken);
            var metadata = await file.GetMetadataAsync(ctx.CancellationToken);

            gFileItem fileitem = new gFileItem()
            {
                Id = Helpers.GenerateUniqueId(),
                TusId = ctx.FileId,
                Size = uint.Parse(new FileInfo(Path.Combine(_uploadOpts.UploadPath, file.Id)).Length.ToString()),
                Name = metadata["name"].GetString(Encoding.UTF8),
                MimeType = metadata["contentType"].GetString(Encoding.UTF8),
                // file with no folderid is placed in the virtual root folder
                FolderId = metadata["folderId"].GetString(Encoding.UTF8) == "root"
                                                                                ? null
                                                                                : metadata["folderId"].GetString(Encoding.UTF8),
                Extension = Helpers.GetFileExtension(metadata["contentType"].GetString(Encoding.UTF8)),
                StorageServerId = metadata["serverId"].GetString(Encoding.UTF8),
                CreatedAt = Timestamp.FromDateTime(DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc)),
                LastModified = Timestamp.FromDateTime(DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc)),
                Status = Protos.ItemStatus.Visible
            };

            // send the uploaded file info to the main app
            gFileItemResponse response = await _storageClient.SaveFileAsync(new gFileItemRequest() { FileItem = fileitem });
            if (response == null)
            {
                _logger.LogError("No response has been received from the server.", ErrorOrigin.Server);
                return null;
            }
            if (response.Status.Status != Protos.RequestStatus.Success)
            {
                _logger.LogError("An error has been returned from server call: " + response.Status.Message);
                return null;
            }
            return response.FileItem;
        }

        /// <summary>
        /// Moves the uploaded files (the actual file and it's metadata) to the user folder
        /// tus protocol puts the uploaded files into the store, XtraUpload move those files to the user directory
        /// </summary>
        private void MoveFilesToFolder(FileCompleteContext ctx, gFileItem fileItem)
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
            foreach (FileInfo file in directoryInfo.GetFiles(ctx.FileId + "*"))
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
