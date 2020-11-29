using MediatR;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using XtraUpload.Authentication.Service.Common;
using XtraUpload.Domain;

namespace XtraUpload.Authentication.Service
{
    /// <summary>
    /// Gets the loggedin user claims and jwt token
    /// </summary>
    public class GetLogedInUserClaimsHandler : IRequestHandler<GetLogedInUserClaimsQuery, XuIdentityResult>
    {
        readonly IJwtFactory _jwtFactory;
        readonly JwtIssuerOptions _jwtOpts;

        public GetLogedInUserClaimsHandler(IJwtFactory jwtFactory, IOptionsSnapshot<JwtIssuerOptions> jwtOpts)
        {
            _jwtFactory = jwtFactory;
            _jwtOpts = jwtOpts.Value;
        }

        /// <summary>
        /// Get user claims
        /// </summary>
        public async Task<XuIdentityResult> Handle(GetLogedInUserClaimsQuery request, CancellationToken cancellationToken)
        {
            XuIdentityResult Result = new XuIdentityResult
            {
                User = request.User,
                Role = request.RoleClaims.Role,
                ClaimsIdentity = _jwtFactory.GenerateClaimsIdentity(request.User, request.RoleClaims.Claims)
            };

            Result.JwtToken = await GenerateJwt(Result.ClaimsIdentity, request.User, request.RememberMe);

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
