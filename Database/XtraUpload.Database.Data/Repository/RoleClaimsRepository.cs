using System;
using System.Collections.Generic;
using System.Text;
using XtraUpload.Database.Data.Common;
using XtraUpload.Domain;

namespace XtraUpload.Database.Data
{
    public class RoleClaimsRepository : Repository<RoleClaim>, IRoleClaimsRepository
    {
        readonly ApplicationDbContext _context;

        public RoleClaimsRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _context = dbContext;
        }
    }
}
