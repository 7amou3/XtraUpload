using Microsoft.AspNetCore.Identity;

namespace XtraUpload.Domain
{
    public class RoleClaim : IdentityRoleClaim<string>
    {
        public virtual Role Role { get; set; }
    }

}
