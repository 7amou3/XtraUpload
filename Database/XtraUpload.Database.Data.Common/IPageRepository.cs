using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using XtraUpload.Domain;

namespace XtraUpload.Database.Data.Common
{
    public interface IPageRepository : IRepository<Page>
    {
        /// <summary>
        /// Get all pages (for fast retrieval we select only the header, no content is selected)
        /// </summary>
        Task<IEnumerable<PageHeader>> GetPagesHeader(Expression<Func<PageHeader, bool>> predicate);
    }
}
