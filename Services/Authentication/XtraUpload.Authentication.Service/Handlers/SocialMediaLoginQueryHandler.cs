using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using XtraUpload.Authentication.Service.Common;
using XtraUpload.Database.Data.Common;
using XtraUpload.Domain;

namespace XtraUpload.Authentication.Service
{
    /// <summary>
    /// Handles the user authentication or account creation with social media providers (fb, google...)
    /// </summary>
    public class SocialMediaLoginQueryHandler : IRequestHandler<SocialMediaLoginQuery, XuIdentityResult>
    {
        readonly IMediator _mediator;
        readonly IUnitOfWork _unitOfWork;

        public SocialMediaLoginQueryHandler(IUnitOfWork unitOfWork, IMediator mediator)
        {
            _mediator = mediator;
            _unitOfWork = unitOfWork;
        }

        public async Task<XuIdentityResult> Handle(SocialMediaLoginQuery request, CancellationToken cancellationToken)
        {
            XuIdentityResult Result = new XuIdentityResult();

            User userInfo = await _unitOfWork.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            // If user does not exist
            if (userInfo == null)
            {
                // Create the new user
                CreateUserResult userResult = await _mediator.Send(new CreateUserCommand(new User()
                {
                    Email = request.Email,
                    UserName = request.Name,
                    Avatar = request.PhotoUrl,
                    SocialMediaId = request.Id,
                    Password = Helpers.GenerateUniqueId(),
                    Provider = (AuthProvider)Enum.Parse(typeof(AuthProvider), request.Provider),
                }));

                if (userResult.State != OperationState.Success)
                {
                    return OperationResult.CopyResult<XuIdentityResult>(userResult);
                }
                // User created successfully
                userInfo = userResult.User;
            }

            // Check user is not suspended
            if (userInfo.AccountSuspended)
            {
                Result.ErrorContent = new ErrorContent("Your Account Has Been Suspended.", ErrorOrigin.Client);
                return Result;
            }

            return await _mediator.Send(new GetLogedInUserClaimsQuery(userInfo, false));
        }
    }
}
