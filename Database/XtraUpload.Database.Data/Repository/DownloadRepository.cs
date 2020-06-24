using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using XtraUpload.Database.Data.Common;
using XtraUpload.Domain;
using XtraUpload.FileManager.Service.Common;

namespace XtraUpload.Database.Data
{
    public class DownloadRepository : Repository<Download>, IDownloadRepository
    {
        readonly ApplicationDbContext _DbContext;

        public DownloadRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _DbContext = dbContext;
        }

        /// <summary>
        /// Get file by download id
        /// </summary>
        public async Task<DownloadedFileResult> GetDownloadedFile(string downloadId)
        {
            return await _DbContext.Downloads
                        .Include(s => s.File)
                        .Select(s => new DownloadedFileResult() { File = s.File, Download = s })
                        .SingleOrDefaultAsync(s => s.Download.Id == downloadId);
        }
    }
}
