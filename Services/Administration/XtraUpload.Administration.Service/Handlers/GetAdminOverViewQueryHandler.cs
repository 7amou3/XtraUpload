﻿using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using XtraUpload.Administration.Service.Common;
using XtraUpload.Database.Data.Common;
using XtraUpload.Domain;
using XtraUpload.GrpcServices.Common;

namespace XtraUpload.Administration.Service
{
    /// <summary>
    /// An overview of the current stats (users, files, disk..)
    /// </summary>
    public class GetAdminOverViewQueryHandler : IRequestHandler<GetAdminOverViewQuery, AdminOverViewResult>
    {
        readonly IMediator _mediator;
        readonly IUnitOfWork _unitOfWork;
        readonly IStorageHealthClientProxy _storageHealthProxy;
        readonly ILogger<GetAdminOverViewQueryHandler> _logger;
        public GetAdminOverViewQueryHandler(
            IMediator mediator,
            IUnitOfWork unitOfWork,
            IStorageHealthClientProxy storageHealthProxy,
            ILogger<GetAdminOverViewQueryHandler> logger)
        {
            _logger = logger;
            _mediator = mediator;
            _unitOfWork = unitOfWork;
            _storageHealthProxy = storageHealthProxy;
        }
        public async Task<AdminOverViewResult> Handle(GetAdminOverViewQuery request, CancellationToken cancellationToken)
        {
            AdminOverViewResult Result = new AdminOverViewResult();
            // Check date range is valid
            if (request.DateRange.Start.Date >= request.DateRange.End.Date)
            {
                Result.ErrorContent = new ErrorContent("Invalid date range.", ErrorOrigin.Client);
                return Result;
            }

            long freeSpace = 0;
            long totalsize = 0;
            foreach (var server in _storageHealthProxy.GetServersHealth)
            {
                freeSpace += (long)server.StorageInfo.FreeDiskSpace;
                totalsize += (long)server.StorageInfo.UsedDiskSpace;
            }
            
            var userStats = await _mediator.Send(new GetUserStatsQuery(request.DateRange));
            var uploadStats = await _mediator.Send(new GetUploadStatsQuery(request.DateRange));
            var fileStats = await _mediator.Send(new GetFileTypeStatsQuery(request.DateRange));

            Result.UsersCount = userStats.UsersCount;
            Result.FilesCount = uploadStats.FilesCount;
            Result.FileTypesCount = fileStats.FileTypesCount;
            Result.TotalFiles = await _unitOfWork.Files.CountAsync(s => s.Status != ItemStatus.To_Be_Deleted);
            Result.TotalUsers = await _unitOfWork.Users.CountAsync();
            Result.DriveSize = totalsize;
            Result.FreeSpace = freeSpace;

            return Result;
        }
    }
}
