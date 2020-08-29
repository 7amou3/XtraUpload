using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using XtraUpload.Domain;

namespace XtraUpload.Authentication.Service.Common
{
    public interface IJwtFactory
    {
        Task<string> GenerateEncodedToken(string userName, ClaimsIdentity identity);
        ClaimsIdentity GenerateClaimsIdentity(User user, IEnumerable<RoleClaim> claims);
    }
}
