using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using XtraUpload.Database.Data.Common;
using XtraUpload.Domain;
using XtraUpload.GrpcServices.Common;
using XtraUpload.Setting.Service.Common;

namespace XtraUpload.Setting.Service
{
    /// <summary>
    /// Get the upload setting for the current user
    /// </summary>
    public class GetUploadSettingQueryHandler : IRequestHandler<GetUploadSettingQuery, UploadSettingResult>
    {
        readonly IUnitOfWork _unitOfWork;
        readonly ClaimsPrincipal _caller;
        readonly UploadOptions _uploadOpt;
        readonly ICheckClientProxy _checkClientProxy;
        readonly ILogger<GetUploadSettingQueryHandler> _logger;
 
        public GetUploadSettingQueryHandler(
            IUnitOfWork unitOfWork,
            ICheckClientProxy checkClientProxy,
            IOptionsMonitor<UploadOptions> uploadOpt,
            IHttpContextAccessor httpContextAccessor,
            ILogger<GetUploadSettingQueryHandler> logger)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _uploadOpt = uploadOpt.CurrentValue;
            _checkClientProxy = checkClientProxy;
            _caller = httpContextAccessor.HttpContext.User;
        }
        
        public async Task<UploadSettingResult> Handle(GetUploadSettingQuery request, CancellationToken cancellationToken)
        {
            string userId = _caller.GetUserId();

            UploadSettingResult Result = new UploadSettingResult();
            try
            {
                var extensions = await _unitOfWork.FileExtensions.GetAll();
                Result.UsedSpace = await _unitOfWork.Files.SumAsync(s => (s.UserId == userId && s.Status != ItemStatus.To_Be_Deleted), s => s.Size);
                Result.StorageSpace = double.Parse(_caller.Claims.Single(c => c.Type == "StorageSpace").Value);
                Result.ConcurrentUpload = int.Parse(_caller.Claims.Single(c => c.Type == "ConcurrentUpload").Value);
                Result.MaxFileSize = int.Parse(_caller.Claims.Single(c => c.Type == "MaxFileSize").Value);
                Result.ChunkSize = _uploadOpt.ChunkSize * 1024 * 1024;
                Result.FileExtensions = string.Join(", ", extensions.Select(s => s.Name));
                Result.StorageServer = GetStorageServer();
            }
            catch (Exception _ex)
            {
                Result.ErrorContent = new ErrorContent(_ex.Message, ErrorOrigin.Server);
                #region Trace
                _logger.LogError(_ex.Message);
                #endregion
            }

            return Result;
        }
        /// <summary>
        /// Get the least loaded server @, for now we pick a random healthy server 
        /// Todo: Get the least loaded storage server based on the monitoring state 
        /// </summary>
        /// <returns></returns>
        private StorageServer GetStorageServer()
        {
            IEnumerable<StorageServer> servers = _checkClientProxy.GetHealthyServers;
            if (servers.Any())
            {
                Random rand = new Random();
                return servers.ElementAt(rand.Next(servers.Count()));
            }
            else
            {
                _logger.LogWarning("No storage server found to serve user: " + _caller.GetUserId());
                return null;
            }
        }
    }
}
