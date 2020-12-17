using MediatR;
using XtraUpload.Domain;

namespace XtraUpload.Authentication.Service.Common
{
    public class GetLogedInUserClaimsQuery: IRequest<XuIdentityResult>
    {
        public GetLogedInUserClaimsQuery(User user, RoleClaims roleClaims, bool rememberMe)
        {
            User = user;
            RoleClaims = roleClaims;
            RememberMe = rememberMe;
        }
        public User User { get; }
        public RoleClaims RoleClaims { get; }
        public bool RememberMe { get; }
    }
}
