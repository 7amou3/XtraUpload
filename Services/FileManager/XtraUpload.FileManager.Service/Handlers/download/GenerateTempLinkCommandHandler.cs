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

            // Generate the download data to store
            Download download = new Download()
            {
                Id = Helpers.GenerateUniqueId(),
                FileId = request.FileId,
                IpAdress = _clientIp,
                StartedAt = DateTime.Now
            };
            _unitOfWork.Downloads.Add(download);

            // Try to save in db
            Result = await _unitOfWork.CompleteAsync(Result);
            if (Result.State == OperationState.Success)
            {
                Result.FileDownload = download;
            }

            return Result;
        }
    }
}
