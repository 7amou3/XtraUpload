using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XtraUpload.gRPCServer
{
    public class gFileStorageService : gFileStorage.gFileStorageBase
    {
        [Authorize]
        public override Task<gUser> GetUser(gRequest request, ServerCallContext context)
        {
            gUser user = new gUser()
            {
                Id = context.GetHttpContext().User.Claims.FirstOrDefault(c => c.Type == "id")?.Value
            };
            return Task.FromResult(user);
        }
    }
}
