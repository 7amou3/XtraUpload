using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using XtraUpload.Domain;

namespace XtraUpload.Authentication.Service.Common
{
    public class GetLogedInUserClaimsQuery: IRequest<XuIdentityResult>
    {
        public GetLogedInUserClaimsQuery(User user, bool rememberMe)
        {
            User = user;
            RememberMe = rememberMe;
        }
        public User User { get; }
        public bool RememberMe { get; }
    }
}
