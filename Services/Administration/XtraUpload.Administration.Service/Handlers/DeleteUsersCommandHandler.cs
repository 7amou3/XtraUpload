using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using XtraUpload.Administration.Service.Common;
using XtraUpload.Database.Data.Common;
using XtraUpload.Domain;

namespace XtraUpload.Administration.Service
{
    /// <summary>
    /// Delete a list of users
    /// </summary>
    public class DeleteUsersCommandHandler : IRequestHandler<DeleteUsersCommand, OperationResult>
    {
        readonly IUnitOfWork _unitOfWork;

        public DeleteUsersCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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
            if (files.Any())
            {
                // Mark files for deletions
                foreach (var file in files)
                {
                    file.Status = ItemStatus.To_Be_Deleted;
                }
            }
            // Delete from db
            _unitOfWork.Users.RemoveRange(users);

            // Persist changes
            result = await _unitOfWork.CompleteAsync(result);

            return result;
        }
    }
}
