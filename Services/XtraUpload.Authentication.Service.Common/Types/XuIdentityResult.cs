using System.Security.Claims;
using XtraUpload.Domain;

namespace XtraUpload.Authentication.Service.Common
{
    public class XuIdentityResult: OperationResult
    {
        public User User { get; set; }
        public JwtToken JwtToken { get; set; }
        public ClaimsIdentity ClaimsIdentity { get; set; }
        public Role Role { get; set; }
    }
}
