using Grpc.Core;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        [Authorize]
        public override Task<gUser> GetUser(gRequest request, ServerCallContext context)
        {
            gUser user = new gUser()
            {
                Id = context.GetHttpContext().User.Claims.FirstOrDefault(c => c.Type == "id")?.Value
            };
            return Task.FromResult(user);
        }

        [Authorize]
        public override async Task<gFileItemResponse> SaveFile(gFileItemRequest request, ServerCallContext context)
        {
            FileItem file = request.FileItem.Convert();
            
            var result = await _mediatr.Send(new SaveFileCommand(file));

            return new gFileItemResponse() { FileItem = result.File.Convert() };
        }

        public async override Task<gFileItemResponse> GetFileById(gFileRequest request, ServerCallContext context)
        {
            var result = await _mediatr.Send(new GetFileServerInfoQuery(request.Fileid));

            return new gFileItemResponse() { FileItem = result.File.Convert() };
        }
    }
}
