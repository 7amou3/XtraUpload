using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using XtraUpload.Administration.Service.Common;
using XtraUpload.Domain;
using XtraUpload.WebApp.Common;

namespace XtraUpload.Database.Data.Common
{
    public interface IFileRepository : IRepository<FileItem>
    {
        Task<IEnumerable<ItemCountResult>> FilesCountByDateRange(DateTime start, DateTime end);
        Task<IEnumerable<FileTypesCountResult>> FileTypesByDateRange(DateTime start, DateTime end);
        Task<IEnumerable<FileItemExtended>> GetFiles(PageSearchViewModel model, Expression<Func<FileItem, bool>> searchCriteria);
        Task<IEnumerable<FileItem>> GetExpiredFiles(CancellationToken cancellationToken);
    }
}
