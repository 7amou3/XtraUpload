using MediatR;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using XtraUpload.Administration.Service.Common;
using XtraUpload.Database.Data.Common;
using XtraUpload.Domain;
using XtraUpload.FileManager.Service.Common;

namespace XtraUpload.Administration.Service
{
    /// <summary>
    /// Delete a list of users
    /// </summary>
    public class DeleteUsersCommandHandler : IRequestHandler<DeleteUsersCommand, OperationResult>
    {
        readonly IUnitOfWork _unitOfWork;
        readonly UploadOptions _uploadOpts;

        public DeleteUsersCommandHandler(IUnitOfWork unitOfWork, IOptionsMonitor<UploadOptions> uploadsOpts)
        {
            _unitOfWork = unitOfWork;
            _uploadOpts = uploadsOpts.CurrentValue;
        }

        public async Task<OperationResult> Handle(DeleteUsersCommand request, CancellationToken cancellationToken)
        {
            OperationResult result = new OperationResult();
            IEnumerable<User> users = await _unitOfWork.Users.FindAsync(s => request.UsersId.Contains(s.Id));
            // Check if users exist
            if (!users.Any())
            {
                result.ErrorContent = new ErrorContent("No user found whith the provided Ids.", ErrorOrigin.Client);
                return result;
            }
            // Get users files
            IEnumerable<FileItem> files = await _unitOfWork.Files.FindAsync(s => request.UsersId.Contains(s.UserId));

            // Delete from db
            _unitOfWork.Users.RemoveRange(users);
            _unitOfWork.Files.RemoveRange(files);

            // Persist changes
            result = await _unitOfWork.CompleteAsync(result);

            if (result.State == OperationState.Success)
            {
                // Remove the files from the drive (no need to queue to background thread, because Directory.Delete does not block)
                foreach (var file in files)
                {
                    string folderPath = Path.Combine(_uploadOpts.UploadPath, file.UserId, file.Id);

                    if (Directory.Exists(folderPath))
                    {
                        Directory.Delete(folderPath, true);
                    }
                }
            }

            return result;
        }
    }
}
