using MediatR;
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
    /// <summary>
    /// Increment the download count for the given file
    /// </summary>
    public class IncrementDownloadCountCommandHandler : IRequestHandler<IncrementDownloadCountCommand, OperationResult>
    {
        readonly IUnitOfWork _unitOfWork;
        public IncrementDownloadCountCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<OperationResult> Handle(IncrementDownloadCountCommand request, CancellationToken cancellationToken)
        {
            OperationResult Result = new OperationResult();

            FileItem file = await _unitOfWork.Files.FirstOrDefaultAsync(s => s.Id == request.FileId);
            if (file == null)
            {
                Result.ErrorContent = new ErrorContent("No file with the provided id was found", ErrorOrigin.Client);
                return Result;
            }

            // Increment the count
            file.DownloadCount++;
            file.LastModified = DateTime.UtcNow;
            Result = await _unitOfWork.CompleteAsync(Result);

            return Result;
        }
    }
}
