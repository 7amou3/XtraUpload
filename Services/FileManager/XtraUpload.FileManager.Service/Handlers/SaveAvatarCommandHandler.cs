using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XtraUpload.Database.Data.Common;
using XtraUpload.Domain;
using XtraUpload.Domain.Infra;
using XtraUpload.FileManager.Service.Common;

namespace XtraUpload.FileManager.Service
{
    public class SaveAvatarCommandHandler : IRequestHandler<SaveAvatarCommand, OperationResult>
    {
        readonly IUnitOfWork _unitOfWork;
        readonly ClaimsPrincipal _caller;

        public SaveAvatarCommandHandler(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _caller = httpContextAccessor.HttpContext.User;
        }

        public async Task<OperationResult> Handle(SaveAvatarCommand request, CancellationToken cancellationToken)
        {
            OperationResult Result = new OperationResult();
            // Check if user exist
            User user = await _unitOfWork.Users.FirstOrDefaultAsync(s => s.Id == _caller.GetUserId());
            if (user == null)
            {
                Result.ErrorContent = new ErrorContent("No user with the provided id was found", ErrorOrigin.Client);
                return Result;
            }

            // Save the new url
            user.Avatar = request.AvatarUrl;
            user.LastModified = DateTime.Now;
            Result = await _unitOfWork.CompleteAsync(Result);

            return Result;
        }
    }
}
