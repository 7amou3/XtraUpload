using Grpc.Core;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XtraUpload.Authentication.Service.Common;
using XtraUpload.Domain;
using XtraUpload.FileManager.Service.Common;
using XtraUpload.Protos;

namespace XtraUpload.GrpcServices
{
    /// <summary>
    /// Service definition for file storage server, the client may request to update/delete, whatever operation on a file
    /// </summary>
    public class gFileManagerService : gFileManager.gFileManagerBase
    {
        readonly IMediator _mediatr;

        public gFileManagerService(IMediator mediatr)
        {
            _mediatr = mediatr;
        }
        /// <summary>
        /// Check if the current grpc request is authorized
        /// a jwt should be attached in order for the server to decode it
        /// </summary>
        public override Task<gIsAuthorizedResponse> IsAuthorized(gIsAuthorizedRequest request, ServerCallContext context)
        {
            var authorized = context.GetHttpContext().User.Identity.IsAuthenticated;
            var response = new gIsAuthorizedResponse()
            {
                Status = new gRequestStatus()
                {
                    Status = authorized ? Protos.RequestStatus.Success : Protos.RequestStatus.Failed
                }
            };
            return Task.FromResult(response);
        }

        [Authorize]
        public override async Task<gUserResponse> GetUser(gUserRequest request, ServerCallContext context)
        {   
            var userResult = await _mediatr.Send(new GetUserByIdQuery());
            
            return new gUserResponse() 
            {  
                User = userResult.User.Convert(),
                Status = userResult.Convert()
            };
        }

        /// <summary>
        /// Save the new uploaded file to db
        /// </summary>
        [Authorize]
        public override async Task<gFileItemResponse> SaveFile(gFileItemRequest request, ServerCallContext context)
        {
            var file = request.FileItem.Convert();
            file.UserId = context.GetHttpContext().User.Claims.FirstOrDefault(c => c.Type == "id").Value;

            var saveResult = await _mediatr.Send(new SaveFileCommand(file));

            return new gFileItemResponse() 
            { 
                FileItem = saveResult.File.Convert(),
                Status = saveResult.Convert()
            };
        }

        /// <summary>
        /// Get file info by id
        /// </summary>
        public async override Task<gFileItemResponse> GetFileById(gFileRequest request, ServerCallContext context)
        {
            var result = await _mediatr.Send(new GetFileServerInfoQuery(request.Fileid));

            return new gFileItemResponse() { FileItem = result.File.Convert(), Status = result.Convert() };
        }

        /// <summary>
        /// Get the file to download
        /// </summary>
        public async override Task<gDownloadFileResponse> GetDownloadFile(gDownloadFileRequest request, ServerCallContext context)
        {
            var result = await _mediatr.Send(new GetDownloadByIdQuery(request.DownloadId, request.RequesterAddress));
           
            return new gDownloadFileResponse()
            {
                Status = result.Convert(),
                FileItem = result.File.Convert(),
                DownloadSpeed = result.DownloadSpeed,
            };
        }

        /// <summary>
        /// Returns a list of files in order to delete them from disk
        /// </summary>
        public override async Task<gFilesItemResponse> GetFilesToDelete(gGetFilesToDeleteRequest request, ServerCallContext context)
        {
            var response = new gFilesItemResponse();

            var result = await _mediatr.Send(new GetFilesToDeleteQuery());
            if (result.State == OperationState.Success)
            {
                response.FilesItem.Add(result.Files.Convert());
            }
            response.Status = result.Convert();

            return response;
        }

        /// <summary>
        /// Delete files from db, this request is triggred by a storage-client's job
        /// </summary>
        public override async Task<gDeleteFilesResponse> DeleteFilesFromDb(gDeleteFilesRequest request, ServerCallContext context)
        {
            var result = await _mediatr.Send(new DeleteFileFromDbCommand(request.FilesId.ToList()));

            return new gDeleteFilesResponse() { Status = result.Convert()};
        }
        /// <summary>
        /// Notification received when a file download has completed
        /// </summary>
        public override async Task<gDownloadCompletedResponse> FileDownloadCompleted(gDownloadCompletedRequest request, ServerCallContext context)
        {
            await _mediatr.Send(new IncrementDownloadCountCommand(request.FileId, request.RequesterIp));

            return new gDownloadCompletedResponse();
        }

        /// <summary>
        /// Save the uploaded avatar to db
        /// </summary>
        [Authorize]
        public override async Task<gSaveAvatarResponse> SaveAvatar(gSaveAvatarRequest request, ServerCallContext context)
        {
            var result = await _mediatr.Send(new SaveAvatarCommand(request.AvatarUrl));

            return new gSaveAvatarResponse() { Status = result.Convert() };
        }
    }
}
