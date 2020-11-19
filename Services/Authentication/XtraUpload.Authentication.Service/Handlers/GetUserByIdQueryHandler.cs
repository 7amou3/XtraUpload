using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using XtraUpload.Authentication.Service.Common;
using XtraUpload.Database.Data.Common;
using XtraUpload.Domain;

namespace XtraUpload.Authentication.Service
{
    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, CreateUserResult>
    {
        readonly IUnitOfWork _unitOfWork;
        readonly ClaimsPrincipal _caller;

        public GetUserByIdQueryHandler(IUnitOfWork unitOfWork, IHttpContextAccessor httpContext)
        {
            _unitOfWork = unitOfWork;
            _caller = httpContext.HttpContext.User;
        }
        public async Task<CreateUserResult> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            CreateUserResult Result = new CreateUserResult();

            User user = await _unitOfWork.Users.FirstOrDefaultAsync(s => s.Id == _caller.GetUserId());
            // Check user exist
            if (user == null)
            {
                Result.ErrorContent = new ErrorContent("No user found with the provided id.", ErrorOrigin.Client);
                return Result;
            }

            Result.User = user;
            return Result;
        }
    }
}
