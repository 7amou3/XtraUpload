using MediatR;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XtraUpload.Domain;
using XtraUpload.Protos;
using XtraUpload.StorageManager.Common;

namespace XtraUpload.StorageManager.Service
{
    /// <summary>
    /// Return the full path to a medium or small thumb 
    /// </summary>
    public class GetThumbnailQueryHandler : IRequestHandler<GetThumbnailQuery, AvatarUrlResult>
    {
        readonly UploadOptions _uploadOpt;
        readonly gFileStorage.gFileStorageClient _storageClient;

        public GetThumbnailQueryHandler(gFileStorage.gFileStorageClient storageClient, IOptionsMonitor<UploadOptions> uploadOpt)
        {
            _storageClient = storageClient;
            _uploadOpt = uploadOpt.CurrentValue;
        }
        public async Task<AvatarUrlResult> Handle(GetThumbnailQuery request, CancellationToken cancellationToken)
        {
            AvatarUrlResult Result = new AvatarUrlResult();

            // Get the file
            var fileResponse = await _storageClient.GetFileByIdAsync(new gFileRequest() { Fileid = request.FileId });
            if (fileResponse == null)
            {
                Result.ErrorContent = new ErrorContent("No response has been received from the server.", ErrorOrigin.Server);
                return Result;
            }
            if (fileResponse.Status.Status != Protos.RequestStatus.Success)
            {
                Result.ErrorContent = new ErrorContent(fileResponse.Status.Message, ErrorOrigin.None);
                return Result;
            }
            string filePath = null;
            switch (request.ThumbnailSize)
            {
                case ThumbnailSize.Small:
                    filePath = Path.Combine(_uploadOpt.UploadPath, fileResponse.FileItem.UserId.ToString(), fileResponse.FileItem.Id, fileResponse.FileItem.Id + ".smallthumb.png");
                    break;
                case ThumbnailSize.Medium:
                    filePath = Path.Combine(_uploadOpt.UploadPath, fileResponse.FileItem.UserId.ToString(), fileResponse.FileItem.Id, fileResponse.FileItem.Id + ".mediumthumb.png");
                    break;
                default:
                    break;
            }
            // todo: handle the case where the image has no medium thumb because it's small
            if (!File.Exists(filePath))
            {
                Result.ErrorContent = new ErrorContent("File does not exist on the server, it may be moved or deleted.", ErrorOrigin.Client);
                return Result;
            }

            Result.Url = filePath;

            return Result;
        }
    }
}
