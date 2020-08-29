using XtraUpload.Database.Data.Common;
using XtraUpload.Domain;

namespace XtraUpload.Database.Data
{
    public class PageRepository : Repository<Page>, IPageRepository
    {
        public PageRepository(ApplicationDbContext context): base (context)
        {
        }
    }
}
