using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using XtraUpload.Database.Data.Common;
using XtraUpload.Domain;
using XtraUpload.FileManager.Service.Common;

namespace XtraUpload.FileManager.Service
{
    public class GetDownloadByIdQueryHandler : IRequestHandler<GetDownloadByIdQuery, DownloadedFileResult>
    {
        readonly IMediator _mediatr;
        readonly IUnitOfWork _unitOfWork;
        readonly ClaimsPrincipal _caller;

        public GetDownloadByIdQueryHandler(
            IMediator mediatr,
            IUnitOfWork unitOfWork,
            IHttpContextAccessor httpContextAccessor)
        {
            _mediatr = mediatr;
            _unitOfWork = unitOfWork;
            _caller = httpContextAccessor.HttpContext.User;
        }

        public async Task<DownloadedFileResult> Handle(GetDownloadByIdQuery request, CancellationToken cancellationToken)
        {
            DownloadedFileResult Result = new DownloadedFileResult();

            var dResult = await _unitOfWork.Downloads.GetDownloadedFile(request.DownloadId);
            //Check download exist
            if (dResult.Download == null || dResult.File == null)
            {
                Result.ErrorContent = new ErrorContent("No file with the provided id was found", ErrorOrigin.Client);
                return Result;
            }
            // Check if it's the same requester
            if (!request.RequesterAddress.Contains(dResult.Download.IpAdress))
            {
                Result.ErrorContent = new ErrorContent("Hotlinking disabled by the administrator.", ErrorOrigin.Client);
                return Result;
            }

            // Get the download options
            DownloadOptionsResult options = await _mediatr.Send(new GetDownloadOptionsQuery(_caller.Identity.IsAuthenticated));
            if (options.State != OperationState.Success)
            {
                Result.ErrorContent = new ErrorContent("Error while reading download options", ErrorOrigin.Server);
                return Result;
            }

            // All good, map results
            Result.File = dResult.File;
            Result.Download = dResult.Download;
            Result.DownloadSpeed = options.Speed;

            return Result;
        }
    }
}
