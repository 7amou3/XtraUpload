using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XtraUpload.Database.Data.Common;
using XtraUpload.Domain;
using XtraUpload.Domain.Infra;
using XtraUpload.FileManager.Service.Common;
using System.Linq;

namespace XtraUpload.FileManager.Service
{
    /// <summary>
    /// Generate a download templink
    /// </summary>
    public class GenerateTempLinkCommandHandler : IRequestHandler<GenerateTempLinkCommand, TempLinkResult>
    {
        readonly string _clientIp;
        readonly IUnitOfWork _unitOfWork;

        public GenerateTempLinkCommandHandler(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _clientIp = httpContextAccessor.HttpContext.Request.Host.Host;
        }

        public async Task<TempLinkResult> Handle(GenerateTempLinkCommand request, CancellationToken cancellationToken)
        {
            TempLinkResult Result = new TempLinkResult();

            var files = await _unitOfWork.Files.GetFilesServerInfo(s => s.Id == request.FileId);
            // Check file exist
            if (!files.Any())
            {
                Result.ErrorContent = new ErrorContent("No file with the provided id was found", ErrorOrigin.Client);
                return Result;
            }
            // Generate the download data to store
            Download download = new Download()
            {
                Id = Helpers.GenerateUniqueId(),
                FileId = request.FileId,
                IpAdress = _clientIp,
                StartedAt = DateTime.Now
            };
            await _unitOfWork.Downloads.AddAsync(download);

            // Try to save in db
            Result = await _unitOfWork.CompleteAsync(Result);
            if (Result.State == OperationState.Success)
            {
                Result.FileDownload = download;
                Result.StorageServerAddress = files.ElementAt(0).StorageServer.Address;
            }

            return Result;
        }
    }
}
