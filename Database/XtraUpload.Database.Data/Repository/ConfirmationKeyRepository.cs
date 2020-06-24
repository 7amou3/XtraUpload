using System;
using System.Collections.Generic;
using System.Text;
using XtraUpload.Database.Data.Common;
using XtraUpload.Domain;

namespace XtraUpload.Database.Data
{
    public class ConfirmationKeyRepository : Repository<ConfirmationKey>, IConfirmationKeyRepository
    {
        readonly ApplicationDbContext _context;

        public ConfirmationKeyRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

    }
}
