using XtraUpload.Database.Data.Common;
using XtraUpload.Domain;

namespace XtraUpload.Database.Data
{
    public class ConfirmationKeyRepository : Repository<ConfirmationKey>, IConfirmationKeyRepository
    {   
        public ConfirmationKeyRepository(ApplicationDbContext context) : base(context)
        {
        }

    }
}
