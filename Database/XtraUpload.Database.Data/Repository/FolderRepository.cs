using XtraUpload.Database.Data.Common;
using XtraUpload.Domain;

namespace XtraUpload.Database.Data
{
    public class FolderRepository: Repository<FolderItem>, IFolderRepository
    {
        readonly ApplicationDbContext _context;
        
        public FolderRepository(ApplicationDbContext context): base(context)
        {
            _context = context;
        }
    }
}
