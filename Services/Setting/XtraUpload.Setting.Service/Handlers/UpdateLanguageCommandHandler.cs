using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using XtraUpload.Database.Data.Common;
using XtraUpload.Domain;
using XtraUpload.Setting.Service.Common;

namespace XtraUpload.Setting.Service
{
    /// <summary>
    /// Update the language of the current user
    /// </summary>
    public class UpdateLanguageCommandHandler : IRequestHandler<UpdateLanguageCommand, OperationResult>
    {
        readonly ClaimsPrincipal _caller;
        readonly IUnitOfWork _unitOfWork;

        public UpdateLanguageCommandHandler(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _caller = httpContextAccessor.HttpContext.User;
        }

        public async Task<OperationResult> Handle(UpdateLanguageCommand request, CancellationToken cancellationToken)
        {
            OperationResult Result = new OperationResult();

            var language = await _unitOfWork.Languages.FirstOrDefaultAsync(s => s.Culture.Contains(request.Culture));
            if (language == null)
            {
                Result.ErrorContent = new ErrorContent("The requested language does not exist.", ErrorOrigin.Client);
                return Result;
            }
            var user = await _unitOfWork.Users.FirstOrDefaultAsync(s => s.Id == _caller.GetUserId());
            if (user != null)
            {
                user.LanguageId = language.Id;
            }
            // Save to db
            return await _unitOfWork.CompleteAsync(Result);
        }
    }
}
