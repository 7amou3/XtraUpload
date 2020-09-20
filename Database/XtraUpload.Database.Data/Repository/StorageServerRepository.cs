using XtraUpload.Database.Data.Common;
using XtraUpload.Domain;

namespace XtraUpload.Database.Data
{
    public class StorageServerRepository : Repository<StorageServer>, IStorageServerRepository
    {
        public StorageServerRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
