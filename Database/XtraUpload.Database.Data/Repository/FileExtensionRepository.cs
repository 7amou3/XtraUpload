using XtraUpload.Database.Data.Common;
using XtraUpload.Domain;

namespace XtraUpload.Database.Data
{
    public class FileExtensionRepository : Repository<FileExtension>, IFileExtensionRepository
    {
        public FileExtensionRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
