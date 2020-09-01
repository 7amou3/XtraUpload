using MediatR;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using XtraUpload.Authentication.Service.Common;
using XtraUpload.Database.Data.Common;
using XtraUpload.Domain;

namespace XtraUpload.Authentication.Service
{
    /// <summary>
    /// Gets the loggedin user claims and jwt token
    /// </summary>
    public class GetLogedInUserClaimsHandler : IRequestHandler<GetLogedInUserClaimsQuery, XuIdentityResult>
    {
        readonly IUnitOfWork _unitOfWork;
        readonly IJwtFactory _jwtFactory;
        readonly JwtIssuerOptions _jwtOpts;

        public GetLogedInUserClaimsHandler(IUnitOfWork unitOfWork, IJwtFactory jwtFactory, IOptionsSnapshot<JwtIssuerOptions> jwtOpts)
        {
            _unitOfWork = unitOfWork;
            _jwtFactory = jwtFactory;
            _jwtOpts = jwtOpts.Value;
        }

        /// <summary>
        /// Get user claims
        /// </summary>
        public async Task<XuIdentityResult> Handle(GetLogedInUserClaimsQuery request, CancellationToken cancellationToken)
        {
            XuIdentityResult Result = new XuIdentityResult();
            RoleClaimsResult claimsResult = await _unitOfWork.Users.GetUserRoleClaims(request.User);

            Result.User = request.User;
            Result.Role = claimsResult.Role;
            Result.ClaimsIdentity = _jwtFactory.GenerateClaimsIdentity(Result.User, claimsResult.Claims);
            Result.JwtToken = await GenerateJwt(Result.ClaimsIdentity, Result.User, request.RememberMe);

            return Result;
        }

        /// <summary>
        /// Generate a jwt token
        /// </summary>
        private async Task<JwtToken> GenerateJwt(ClaimsIdentity identity, User user, bool rememberUser)
        {
            if (rememberUser)
            {
                // Rember me = 30 day token validity
                _jwtOpts.ValidFor = 30;
            }
            return new JwtToken
            {
                Token = await _jwtFactory.GenerateEncodedToken(user.UserName, identity),
                Expires_in = (int)_jwtOpts.ValidFor * 24 * 60 * 60
            };
        }

    }
}
