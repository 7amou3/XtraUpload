using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using XtraUpload.Database.Data.Common;
using XtraUpload.Domain;
using XtraUpload.FileManager.Service.Common;

namespace XtraUpload.FileManager.Service
{
    /// <summary>
    /// Request to download a file
    /// </summary>
    public class RequestDownloadQueryHandler : IRequestHandler<RequestDownloadQuery, RequestDownloadResult>
    {
        readonly string _clientIp;
        readonly IMediator _mediatr;
        readonly IUnitOfWork _unitOfWork;
        readonly ClaimsPrincipal _caller;

        public RequestDownloadQueryHandler(IUnitOfWork unitOfWork, IMediator mediatr, IHttpContextAccessor httpContextAccessor)
        {
            _mediatr = mediatr;
            _unitOfWork = unitOfWork;
            _caller = httpContextAccessor.HttpContext.User;
            _clientIp = httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
        }

        public async Task<RequestDownloadResult> Handle(RequestDownloadQuery request, CancellationToken cancellationToken)
        {
            string userId = _caller.GetUserId();

            RequestDownloadResult Result = new RequestDownloadResult()
            {
                File = await _unitOfWork.Files.FirstOrDefaultAsync(s => s.Id == request.FileId && s.Status != ItemStatus.To_Be_Deleted)
            };
            // Check if file exist
            if (Result.File == null)
            {
                Result.ErrorContent = new ErrorContent("No file with the provided id was found", ErrorOrigin.Client);
                return Result;
            }
            // Check file availablability
            if (userId != Result.File.UserId && Result.File.Status != ItemStatus.Visible)
            {
                Result.ErrorContent = new ErrorContent("This file is not available for public downloads", ErrorOrigin.Client);
                return Result;
            }
            // Get the download options
            DownloadOptionsResult options = await _mediatr.Send(new GetDownloadOptionsQuery(_caller.Identity.IsAuthenticated));
            if (options.State != OperationState.Success)
            {
                Result.ErrorContent = new ErrorContent("Error while reading download options", ErrorOrigin.Client);
                return Result;
            }

            // Check if there is already a download in progress and get remaining time
            var currentDownloads = await GetTTW(options.TTW);

            Result.WaitTime = currentDownloads.TotalDownloads > 0
                              ? currentDownloads.TimeToWait
                              : options.WaitTime;

            return Result;
        }

        /// <summary>
        /// Get time to wait before a new download is permitted
        /// </summary>
        private async Task<GetTTWResult> GetTTW(int TTW)
        {
            // count the total current client downloads
            var query = await _unitOfWork.Downloads.FindAsync(s => s.IpAdress == _clientIp
                                                                    && s.StartedAt.AddSeconds(TTW) > DateTime.Now);

            GetTTWResult Result = new GetTTWResult
            {
                TotalDownloads = query.Count()
            };
            if (Result.TotalDownloads != 0)
            {
                var elapsedTime = (query.ElementAt(query.Count() - 1).StartedAt.AddSeconds(TTW) - DateTime.Now).TotalSeconds;
                Result.TimeToWait = elapsedTime > 0 ? (int)Math.Round(elapsedTime) : 0;
            }

            return Result;
        }
    }
}
