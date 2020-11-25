using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using XtraUpload.Authentication.Service.Common;
using XtraUpload.Database.Data.Common;
using XtraUpload.Domain;

namespace XtraUpload.Authentication.Service
{
    /// <summary>
    /// Handler to check user identity:
    /// - Standard authentication (user provide password and username)
    /// </summary>
    public class StandardLoginQueryHandler : IRequestHandler<StandardLoginQuery, XuIdentityResult>
    {
        readonly IMediator _mediator;
        readonly IUnitOfWork _unitOfWork;
        readonly ILogger<StandardLoginQueryHandler> _logger;

        public StandardLoginQueryHandler(IUnitOfWork unitOfWork, IMediator mediator, ILogger<StandardLoginQueryHandler> logger)
        {
            _logger = logger;
            _mediator = mediator;
            _unitOfWork = unitOfWork;
        }

        public async Task<XuIdentityResult> Handle(StandardLoginQuery credentials, CancellationToken cancellationToken)
        {
            XuIdentityResult Result = new XuIdentityResult();

            User user = await _unitOfWork.Users.GetUser(u => u.Email == credentials.Email);
            // Check the user exist
            if (user == null)
            {
                Result.ErrorContent = new ErrorContent("No user found with the provided email.", ErrorOrigin.Client);
                return Result;
            }
            // Check user is not suspended
            if (user.AccountSuspended)
            {
                Result.ErrorContent = new ErrorContent("Your Account Has Been Suspended.", ErrorOrigin.Client);
                return Result;
            }
            // Check the password
            if (!Helpers.CheckPassword(credentials.Password, user.Password))
            {
                Result.ErrorContent = new ErrorContent("Email or Password does not match", ErrorOrigin.Client);
                return Result;
            }

            // Get user claims
            Result = await _mediator.Send(new GetLogedInUserClaimsQuery(user, credentials.RememberMe));

            #region Trace
            if (Result.State != OperationState.Success)
            {
                _logger.LogError(Result.ErrorContent.ToString());
            }
            #endregion

            return Result;
        }
    }
}
