using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XtraUpload.Database.Data.Common;
using XtraUpload.Domain;
using XtraUpload.FileManager.Service.Common;

namespace XtraUpload.FileManager.Service
{
    public class GetFilesToDeleteQueryHandler : IRequestHandler<GetFilesToDeleteQuery, GetFilesResult>
    {
        readonly IUnitOfWork _unitOfWork;
        readonly HttpContext _httpContext;

        public GetFilesToDeleteQueryHandler(IUnitOfWork unitOfWork, IHttpContextAccessor contextAccessor)
        {
            _unitOfWork = unitOfWork;
            _httpContext = contextAccessor.HttpContext;
        }
        public async Task<GetFilesResult> Handle(GetFilesToDeleteQuery request, CancellationToken cancellationToken)
        {
            string hostname = _httpContext.Request.Host.Host;
            GetFilesResult Result = new GetFilesResult
            {
                Files = await _unitOfWork.Files.GetFilesServerInfo(s => s.Status == ItemStatus.To_Be_Deleted
                                                                            && s.StorageServer.Address.Contains(hostname))
            };

            return Result;
        }
    }
}
