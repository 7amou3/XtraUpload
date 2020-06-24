using System.Threading.Tasks;
using XtraUpload.Administration.Service.Common;
using XtraUpload.Database.Data.Common;
using XtraUpload.Domain;

namespace XtraUpload.Database.Data
{
    public class RoleRepository : Repository<Role>, IRoleRepository
    {
        readonly ApplicationDbContext _context;

        public RoleRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _context = dbContext;
        }
    }
}
