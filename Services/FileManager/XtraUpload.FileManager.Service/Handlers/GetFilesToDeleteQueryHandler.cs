﻿using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
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
        readonly ILogger<GetFilesToDeleteQueryHandler> _logger;
        public GetFilesToDeleteQueryHandler(
            IUnitOfWork unitOfWork,
            IHttpContextAccessor contextAccessor,
            ILogger<GetFilesToDeleteQueryHandler> logger)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _httpContext = contextAccessor.HttpContext;
        }
        public async Task<GetFilesResult> Handle(GetFilesToDeleteQuery request, CancellationToken cancellationToken)
        {
            List<FileItem> filesToDelete = new List<FileItem>();
            string remoteServerIp = _httpContext.Connection.RemoteIpAddress.ToString();
            _logger.LogInformation("Getting a list of files to remove from server: " + remoteServerIp);
            IEnumerable<FileItem> files = await _unitOfWork.Files.GetFilesServerInfo(s => s.Status == ItemStatus.To_Be_Deleted);
            foreach (FileItem file in files)
            {
                if (Helpers.HostnameToIp(file.StorageServer.Address).Contains(remoteServerIp))
                {
                    filesToDelete.Add(file);
                }
            }
            GetFilesResult Result = new GetFilesResult()
            {
                Files = filesToDelete
            };

            _logger.LogInformation(Result.Files.Count() + " Files to be deleted from: "+ remoteServerIp);
            return Result;
        }

    }
}
