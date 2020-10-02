using Grpc.Core;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
    public class gFileStorageService : gFileStorage.gFileStorageBase
    {
        readonly IMediator _mediatr;

        public gFileStorageService(IMediator mediatr)
        {
            _mediatr = mediatr;
        }

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
            var id = context.GetHttpContext().User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            
            var userResult = await _mediatr.Send(new GetUserByIdQuery(id));
            
            return new gUserResponse() 
            {  
                User = userResult.User.Convert(),
                Status = userResult.Convert()
            };
        }

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

        public async override Task<gFileItemResponse> GetFileById(gFileRequest request, ServerCallContext context)
        {
            var result = await _mediatr.Send(new GetFileServerInfoQuery(request.Fileid));

            return new gFileItemResponse() { FileItem = result.File.Convert() };
        }

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
    }
}
