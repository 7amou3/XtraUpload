using MediatR;
using Microsoft.Extensions.Options;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using XtraUpload.Database.Data.Common;
using XtraUpload.Domain;
using XtraUpload.FileManager.Service.Common;

namespace XtraUpload.FileManager.Service
{
    /// <summary>
    /// Delete the file from the drive and db
    /// </summary>
    public class DeleteFileCommandHandler : IRequestHandler<DeleteFileCommand, DeleteFileResult>
    {
        #region Fields
        readonly IUnitOfWork _unitOfWork;
        readonly UploadOptions _uploadOpt;
        #endregion

        #region Constructor
        public DeleteFileCommandHandler(IUnitOfWork unitOfWork, IOptionsMonitor<UploadOptions> uploadOpt)
        {
            _unitOfWork = unitOfWork;
            _uploadOpt = uploadOpt.CurrentValue;
        }
        #endregion

        #region Handler
        public async Task<DeleteFileResult> Handle(DeleteFileCommand request, CancellationToken cancellationToken)
        {
            DeleteFileResult Result = new DeleteFileResult();
            FileItem file = await _unitOfWork.Files.FirstOrDefaultAsync(s => s.Id == request.FileId);

            // Check if file exist
            if (file == null)
            {
                Result.ErrorContent = new ErrorContent("No file with the provided id was found", ErrorOrigin.Client);
                return Result;
            }

            // Remove file from collection
            _unitOfWork.Files.Remove(file);

            // Save to db
            Result = await _unitOfWork.CompleteAsync(Result);
            if (Result.State == OperationState.Success)
            {
                // Delete from disk (no need to queue to background thread, because Directory.Delete does not block)
                string folderPath = Path.Combine(_uploadOpt.UploadPath, file.UserId, file.Id);
                if (Directory.Exists(folderPath))
                {
                    Directory.Delete(folderPath, true);
                }
                // Append new data
                Result.File = file;
            }

            return Result;
        }
        #endregion
    }
}
