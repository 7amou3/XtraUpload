using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XtraUpload.Administration.Service.Common;
using XtraUpload.Database.Data.Common;
using XtraUpload.Domain;

namespace XtraUpload.Administration.Service
{
    /// <summary>
    /// Edit a user
    /// </summary>
    public class EditUserCommandHandler : IRequestHandler<EditUserCommand, EditUserResult>
    {
        readonly IUnitOfWork _unitOfWork;
        public EditUserCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<EditUserResult> Handle(EditUserCommand request, CancellationToken cancellationToken)
        {
            EditUserResult result = new EditUserResult();
            // Check email is not duplicated
            User duplicatedUser = await _unitOfWork.Users.FirstOrDefaultAsync(s => s.Email == request.Email && s.Id != request.Id);
            if (duplicatedUser != null)
            {
                result.ErrorContent = new ErrorContent("A user with the provided email already exists.", ErrorOrigin.Client);
                return result;
            }
            // Check user exists
            User user = await _unitOfWork.Users.FirstOrDefaultAsync(s => s.Id == request.Id);
            if (user == null)
            {
                result.ErrorContent = new ErrorContent("No user found with provided Id.", ErrorOrigin.Client);
                return result;
            }

            if (!string.IsNullOrEmpty(request.NewPassword))
            {
                user.Password = Helpers.HashPassword(request.NewPassword);
            }
            user.Email = request.Email;
            user.RoleId = request.RoleId;
            user.UserName = request.UserName;
            user.EmailConfirmed = request.EmailConfirmed;
            user.AccountSuspended = request.SuspendAccount;
            user.LastModified = DateTime.UtcNow;

            // Persist changes
            return await _unitOfWork.CompleteAsync(result);
        }
    }
}
