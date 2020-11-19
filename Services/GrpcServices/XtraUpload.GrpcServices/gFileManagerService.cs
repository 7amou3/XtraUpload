using Grpc.Core;
using MediatR;
using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize(AuthenticationSchemes = CertificateAuthenticationDefaults.AuthenticationScheme)]
    public class gFileManagerService : gFileManager.gFileManagerBase
    {
        readonly IMediator _mediatr;

        public gFileManagerService(IMediator mediatr)
        {
            _mediatr = mediatr;
        }
        /// <summary>
        /// Check if the jwt is valid thus the user is authenticated
        /// </summary>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "User")]
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

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "User")]
        public override async Task<gUserResponse> GetUser(gUserRequest request, ServerCallContext context)
        {
            var response = new gUserResponse();
            var result = await _mediatr.Send(new GetUserByIdQuery());

            response.Status = result.Convert();
            if (result.State == OperationState.Success)
            {
                response.User = result.User.Convert();
            }

            return response;
        }

        /// <summary>
        /// Save the new uploaded file to db
        /// </summary>
        public override async Task<gFileItemResponse> SaveFile(gFileItemRequest request, ServerCallContext context)
        {
            var response = new gFileItemResponse();
            var result = await _mediatr.Send(new SaveFileCommand(request.FileItem.Convert()));
            
            response.Status = result.Convert();
            if (result.State == OperationState.Success)
            {
                response.FileItem = result.File.Convert();
            }

            return response;
        }

        /// <summary>
        /// Get file info by id
        /// </summary>
        public async override Task<gFileItemResponse> GetFileById(gFileRequest request, ServerCallContext context)
        {
            var response = new gFileItemResponse();
            var result = await _mediatr.Send(new GetFileServerInfoQuery(request.Fileid));

            response.Status = result.Convert();
            if (result.State == OperationState.Success)
            {
                response.FileItem = result.File.Convert();
            }

            return response;
        }

        /// <summary>
        /// Get the file to download
        /// </summary>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "User")]
        public async override Task<gDownloadFileResponse> GetDownloadFile(gDownloadFileRequest request, ServerCallContext context)
        {
            var response = new gDownloadFileResponse();
            var result = await _mediatr.Send(new GetDownloadByIdQuery(request.DownloadId, request.RequesterAddress));
            
            response.Status = result.Convert();
            if (result.State == OperationState.Success)
            {
                response.FileItem = result.File.Convert();
                response.DownloadSpeed = result.DownloadSpeed;
            }

            return response;
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
        public override async Task<gSaveAvatarResponse> SaveAvatar(gSaveAvatarRequest request, ServerCallContext context)
        {
            var result = await _mediatr.Send(new SaveAvatarCommand(request.UserId, request.AvatarUrl));

            return new gSaveAvatarResponse() { Status = result.Convert() };
        }
    }
}
