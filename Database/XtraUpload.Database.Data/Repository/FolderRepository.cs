using XtraUpload.Database.Data.Common;
using XtraUpload.Domain;

namespace XtraUpload.Database.Data
{
    public class FolderRepository: Repository<FolderItem>, IFolderRepository
    {
        public FolderRepository(ApplicationDbContext context): base(context)
        {
        }
    }
}
