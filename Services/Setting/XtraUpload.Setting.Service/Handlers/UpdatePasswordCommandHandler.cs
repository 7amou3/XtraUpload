using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using XtraUpload.Database.Data.Common;
using XtraUpload.Domain;
using XtraUpload.Domain.Infra;
using XtraUpload.Setting.Service.Common;

namespace XtraUpload.Setting.Service
{
    public class UpdatePasswordCommandHandler : IRequestHandler<UpdatePasswordCommand, UpdatePasswordResult>
    {
        readonly ClaimsPrincipal _caller;
        readonly IUnitOfWork _unitOfWork;
        
        public UpdatePasswordCommandHandler(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _caller = httpContextAccessor.HttpContext.User;
        }
        
        public async Task<UpdatePasswordResult> Handle(UpdatePasswordCommand request, CancellationToken cancellationToken)
        {
            string userId = _caller.GetUserId();
            UpdatePasswordResult Result = new UpdatePasswordResult();
            User user = await _unitOfWork.Users.FirstOrDefaultAsync(s => s.Id == userId);

            // Check user exist
            if (user == null)
            {
                Result.ErrorContent = new ErrorContent("No user found with the provided email.", ErrorOrigin.Client);
                return Result;
            }

            // Check password match
            if (!Helpers.CheckPassword(request.OldPassword, user.Password))
            {
                Result.ErrorContent = new ErrorContent("Your current password is incorrect.", ErrorOrigin.Client);
                return Result;
            }

            // Update the password in the collection
            user.Password = Helpers.HashPassword(request.NewPassword);
            user.LastModified = DateTime.Now;

            // Save to db
            return await _unitOfWork.CompleteAsync(Result);
        }
    }
}
