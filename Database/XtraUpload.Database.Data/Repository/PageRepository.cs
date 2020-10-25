using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using XtraUpload.Database.Data.Common;
using XtraUpload.Domain;

namespace XtraUpload.Database.Data
{
    public class PageRepository : Repository<Page>, IPageRepository
    {
        readonly ApplicationDbContext _context;
        public PageRepository(ApplicationDbContext context): base (context)
        {
            _context = context;
        }
        /// <summary>
        /// Get all pages (for fast retrieval we select only the header, no content is selected)
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<PageHeader>> GetPagesHeader(Expression<Func<PageHeader, bool>> predicate)
        {
            if (predicate == null)
            {
                predicate = p => true;
            }
            var result = await _context.Pages
                .Where(predicate)
                .OrderByDescending(s => s.CreatedAt)
                .Select(s => new PageHeader()
                {
                    Id = s.Id,
                    Name = s.Name,
                    Url = s.Url,
                    VisibleInFooter = s.VisibleInFooter,
                    CreatedAt = s.CreatedAt,
                    UpdatedAt = s.UpdatedAt,
                }).ToListAsync();
            return result;
        }
    }
}
