using MediatR;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using XtraUpload.Database.Data.Common;
using XtraUpload.Domain;
using XtraUpload.FileManager.Service.Common;

namespace XtraUpload.FileManager.Service
{
    /// <summary>
    /// Delete files from db
    /// </summary>
    public class DeleteFileFromDbCommandHandler : IRequestHandler<DeleteFileFromDbCommand, DeleteFileResult>
    {
        readonly IUnitOfWork _unitOfWork;
        
        public DeleteFileFromDbCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
       
        public async Task<DeleteFileResult> Handle(DeleteFileFromDbCommand request, CancellationToken cancellationToken)
        {
            DeleteFileResult Result = new DeleteFileResult();
            IEnumerable<FileItem> files = await _unitOfWork.Files.FindAsync(s => request.FilesId.Contains(s.Id));

            // Check if files exist
            if (!files.Any())
            {
                Result.ErrorContent = new ErrorContent("No files with the provided id are found", ErrorOrigin.Client);
                return Result;
            }

            // Remove file from collection
            _unitOfWork.Files.RemoveRange(files);

            // Save to db
            Result = await _unitOfWork.CompleteAsync(Result);
            if (Result.State == OperationState.Success)
            {
                // Append new data
                Result.Files = files;
            }

            return Result;
        }
    }
}
