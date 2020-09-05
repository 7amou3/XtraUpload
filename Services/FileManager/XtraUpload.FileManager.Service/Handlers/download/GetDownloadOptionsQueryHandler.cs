using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XtraUpload.Database.Data.Common;
using XtraUpload.Domain;
using XtraUpload.FileManager.Service.Common;

namespace XtraUpload.FileManager.Service.Handlers
{
    /// <summary>
    /// Get download option of the requested profil
    /// </summary>
    public class GetDownloadOptionsQueryHandler : IRequestHandler<GetDownloadOptionsQuery, DownloadOptionsResult>
    {
        readonly IUnitOfWork _unitOfWork;
        readonly ClaimsPrincipal _caller;
        readonly ILogger<GetDownloadOptionsQueryHandler> _logger;

        public GetDownloadOptionsQueryHandler(
            IUnitOfWork unitOfWork, 
            IHttpContextAccessor httpContextAccessor,
            ILogger<GetDownloadOptionsQueryHandler> logger)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _caller = httpContextAccessor.HttpContext.User;
        }

        public async Task<DownloadOptionsResult> Handle(GetDownloadOptionsQuery request, CancellationToken cancellationToken)
        {
            DownloadOptionsResult option = new DownloadOptionsResult()
            {
                Speed = 500,
                TTW = 5,
                WaitTime = 60
            };
            // Authenticated user
            if (request.AuthenticatedUser)
            {
                if (double.TryParse(_caller.Claims.Single(c => c.Type == "DownloadSpeed").Value, out double downloadSpeed))
                {
                    option.Speed = downloadSpeed;
                }
                if (int.TryParse(_caller.Claims.Single(c => c.Type == "DownloadTTW").Value, out int downloadTTW))
                {
                    option.TTW = downloadTTW;
                }
                if (int.TryParse(_caller.Claims.Single(c => c.Type == "WaitTime").Value, out int waitTime))
                {
                    option.WaitTime = waitTime;
                }
            }
            // Guest user
            else
            {
                IEnumerable<RoleClaim> roleClaims = await _unitOfWork.RoleClaims.FindAsync(s => s.RoleId == "3"); // Guest have Id = 3 in RoleClaims table

                if (roleClaims.Any())
                {
                    if (double.TryParse(roleClaims.First(s => s.ClaimType == XtraUploadClaims.DownloadSpeed.ToString()).ClaimValue, out double downloadSpeed))
                    {
                        option.Speed = downloadSpeed;
                    }
                    if (int.TryParse(roleClaims.First(s => s.ClaimType == XtraUploadClaims.DownloadTTW.ToString()).ClaimValue, out int downloadTTW))
                    {
                        option.TTW = downloadTTW;
                    }
                    if (int.TryParse(roleClaims.First(s => s.ClaimType == XtraUploadClaims.WaitTime.ToString()).ClaimValue, out int downloadWaitTime))
                    {
                        option.WaitTime = downloadWaitTime;
                    }
                }
                else
                {
                    #region Trace
                    _logger.LogError("No role claims found for guest user.");
                    #endregion
                }
            }

            return option;
        }
    }
}
