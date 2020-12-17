using XtraUpload.Database.Data.Common;
using XtraUpload.Domain;

namespace XtraUpload.Database.Data
{
    public class LanguageRepository: Repository<Language>, ILanguageRepository
    {
        public LanguageRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
