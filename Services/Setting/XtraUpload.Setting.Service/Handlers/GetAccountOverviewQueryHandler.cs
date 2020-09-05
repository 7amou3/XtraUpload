using MediatR;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using XtraUpload.Database.Data.Common;
using XtraUpload.Domain;
using XtraUpload.Domain.Infra;
using XtraUpload.Setting.Service.Common;

namespace XtraUpload.Setting.Service
{
    /// <summary>
    /// Gets the user account overview
    /// </summary>
    public class GetAccountOverviewQueryHandler : IRequestHandler<GetAccountOverviewQuery, AccountOverviewResult>
    {
        readonly IMediator _mediatr;
        readonly IUnitOfWork _unitOfWork;
        readonly ClaimsPrincipal _caller;
        
        public GetAccountOverviewQueryHandler(IUnitOfWork unitOfWork, IMediator mediatr, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _mediatr = mediatr;
            _caller = httpContextAccessor.HttpContext.User;
        }
        
        public async Task<AccountOverviewResult> Handle(GetAccountOverviewQuery request, CancellationToken cancellationToken)
        {
            string userId = _caller.GetUserId();

            AccountOverviewResult Result = new AccountOverviewResult
            {
                // Read the download setting from jwt token
                DownloadSetting = new DownloadSettingResult()
                {
                    DownloadSpeed = int.Parse(_caller.Claims.Single(c => c.Type == "DownloadSpeed").Value) * 1024,
                    DownloadTTW = int.Parse(_caller.Claims.Single(c => c.Type == "DownloadTTW").Value),
                    FileExpiration = int.Parse(_caller.Claims.Single(c => c.Type == "FileExpiration").Value),
                    TimeToWait = int.Parse(_caller.Claims.Single(c => c.Type == "WaitTime").Value),
                },

                // Get the upload settings
                UploadSetting = await _mediatr.Send(new GetUploadSettingQuery())
            };

            if (Result.UploadSetting.State != OperationState.Success)
            {
                Result = OperationResult.CopyResult<AccountOverviewResult>(Result.UploadSetting);
                return Result;
            }

            // Get the files stats
            Result.FilesStats = new FilesStatsResult()
            {
                TotalDownloads = await _unitOfWork.Files.SumAsync(s => s.UserId == userId, s => s.DownloadCount),
                TotalFiles = await _unitOfWork.Files.CountAsync(s => s.UserId == userId)
            };

            // Get user info
            Result.User = await _unitOfWork.Users.FirstOrDefaultAsync(s => s.Id == userId);

            return Result;
        }
    }
}
