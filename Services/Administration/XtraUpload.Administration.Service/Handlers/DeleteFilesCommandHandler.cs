using MediatR;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XtraUpload.Administration.Service.Common;
using XtraUpload.Database.Data.Common;
using XtraUpload.Domain;
using XtraUpload.FileManager.Service.Common;

namespace XtraUpload.Administration.Service
{
    /// <summary>
    /// Delete file(s) by id
    /// </summary>
    public class DeleteFilesCommandHandler : IRequestHandler<DeleteFilesCommand, DeleteFilesResult>
    {
        readonly IUnitOfWork _unitOfWork;
        readonly UploadOptions _uploadOpts;

        public DeleteFilesCommandHandler(IUnitOfWork unitOfWork, IOptionsMonitor<UploadOptions> uploadsOpts)
        {
            _unitOfWork = unitOfWork;
            _uploadOpts = uploadsOpts.CurrentValue;
        }

        public async Task<DeleteFilesResult> Handle(DeleteFilesCommand request, CancellationToken cancellationToken)
        {
            DeleteFilesResult result = new DeleteFilesResult();
            IEnumerable<FileItem> files = await _unitOfWork.Files.FindAsync(s => request.FilesId.Contains(s.Id));
            if (files != null && files.Any())
            {
                // Remove from collection
                _unitOfWork.Files.RemoveRange(files);

                // Save to db
                result = await _unitOfWork.CompleteAsync(result);
                if (result.State == OperationState.Success)
                {
                    // Delete from disk (no need to queue to background thread, because Directory.Delete does not block)
                    foreach (FileItem file in files)
                    {
                        string folderPath = Path.Combine(_uploadOpts.UploadPath, file.UserId, file.Id);
                        if (Directory.Exists(folderPath))
                        {
                            Directory.Delete(folderPath, true);
                        }
                    }
                }
            }
            else
            {
                result.ErrorContent = new ErrorContent($"Please check the provided id(s)", ErrorOrigin.Server);
            }

            result.Files = files;
            return result;
        }
    }
}
