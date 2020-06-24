using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using XtraUpload.Administration.Service.Common;
using XtraUpload.Database.Data.Common;
using XtraUpload.Domain;
using XtraUpload.ServerApp.Common;

namespace XtraUpload.Database.Data
{
    public class FileRepository : Repository<FileItem>, IFileRepository
    {
        readonly ApplicationDbContext _context;
  
        public FileRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _context = dbContext;
        }

        public async Task<IEnumerable<ItemCountResult>> FilesCountByDateRange(DateTime start, DateTime end)
        {
            var query = _context.Files
                        .Where(s => s.CreatedAt >= start && s.CreatedAt <= end)
                        .GroupBy(f => f.CreatedAt.Date)
                        .OrderBy(s => s.Key)
                        .Select(s => new ItemCountResult 
                        {
                            Date = s.Key.Date,
                            ItemCount = s.Count() 
                        });

            return await query.ToListAsync();
        }
        
        public async Task<IEnumerable<FileTypesCountResult>> FileTypesByDateRange(DateTime start, DateTime end)
        {
            var query = _context.Files
                        .Where(s => s.CreatedAt >= start && s.CreatedAt <= end)
                        .GroupBy(f => f.Extension)
                        .OrderBy(s => s.Key)
                        .Select(s => new FileTypesCountResult
                        {
                            Extension = s.Key,
                            ItemCount = s.Count() 
                        });

            return await query.ToListAsync(); 
        }
        
        public async Task<IEnumerable<FileItemExtended>> GetFiles(PageSearchViewModel model, Expression<Func<FileItem, bool>> searchCriteria)
        {
            var query = _context.Files
                            .Include(s => s.User)
                            .Where(searchCriteria)
                            .OrderBy(s => s.CreatedAt)
                            .Skip(model.PageIndex * model.PageSize)
                            .Take(model.PageSize)
                            .Select(s => new FileItemExtended()
                            {
                                Id = s.Id,
                                UserName = s.User.UserName,
                                Name = s.Name,
                                Extension = s.Extension,
                                Size = s.Size,
                                DownloadCount = s.DownloadCount,
                                CreatedAt = s.CreatedAt,
                            });

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<FileItem>> GetExpiredFiles()
        {
            List<RoleClaim> rcList = await _context.RoleClaims
                                    .Where(s => s.ClaimType == XtraUploadClaims.FileExpiration.ToString())
                                    .Where(s => s.ClaimValue != "0")
                                    .ToListAsync();

            List<FileItem> expiredFiles = new List<FileItem>();
            rcList.ForEach(userGroup =>
            {
                var res = _context.Files
                            .Include(u => u.User)
                            .Where(s => userGroup.RoleId == s.User.RoleId)
                            .Where(s => s.LastModified < DateTime.Now.AddDays(- int.Parse(userGroup.ClaimValue)))
                            .ToList();

                expiredFiles.AddRange(res);
            });
            return expiredFiles;
        }
    }
}
