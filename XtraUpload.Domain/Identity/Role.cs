using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace XtraUpload.Domain
{
    public class Role : IdentityRole
    {
        public Role()
        {
            Users = new HashSet<User>();
            RoleClaims = new HashSet<RoleClaim>();
        }
        public bool IsDefault { get; set; }

        public virtual ICollection<User> Users { get; set; }
        public virtual ICollection<RoleClaim> RoleClaims { get; set; }
    }
}
