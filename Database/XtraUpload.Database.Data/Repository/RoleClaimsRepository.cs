using XtraUpload.Database.Data.Common;
using XtraUpload.Domain;

namespace XtraUpload.Database.Data
{
    public class RoleClaimsRepository : Repository<RoleClaim>, IRoleClaimsRepository
    {
        public RoleClaimsRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
