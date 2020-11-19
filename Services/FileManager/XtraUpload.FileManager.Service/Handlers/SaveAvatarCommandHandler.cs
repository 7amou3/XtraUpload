using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using XtraUpload.Database.Data.Common;
using XtraUpload.Domain;
using XtraUpload.FileManager.Service.Common;

namespace XtraUpload.FileManager.Service
{
    public class SaveAvatarCommandHandler : IRequestHandler<SaveAvatarCommand, OperationResult>
    {
        readonly IUnitOfWork _unitOfWork;

        public SaveAvatarCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<OperationResult> Handle(SaveAvatarCommand request, CancellationToken cancellationToken)
        {
            OperationResult Result = new OperationResult();
            // Check if user exist
            User user = await _unitOfWork.Users.FirstOrDefaultAsync(s => s.Id == request.UserId);
            if (user == null)
            {
                Result.ErrorContent = new ErrorContent("No user with the provided id was found", ErrorOrigin.Client);
                return Result;
            }

            // Save the new url
            user.Avatar = request.AvatarUrl;
            user.LastModified = DateTime.UtcNow;
            Result = await _unitOfWork.CompleteAsync(Result);

            return Result;
        }
    }
}
