using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using XtraUpload.Administration.Service.Common;
using XtraUpload.Domain;

namespace XtraUpload.Database.Data.Common
{
    public interface IFileRepository : IRepository<FileItem>
    {
        Task<IEnumerable<FileItem>> GetFilesServerInfo(Expression<Func<FileItem, bool>> criteria);
        Task<IEnumerable<ItemCountResult>> FilesCountByDateRange(DateTime start, DateTime end);
        Task<IEnumerable<FileTypesCountResult>> FileTypesByDateRange(DateTime start, DateTime end);
        Task<IEnumerable<FileItemExtended>> GetFiles(PageSearchModel model, Expression<Func<FileItem, bool>> searchCriteria);
        Task<IEnumerable<FileItem>> GetExpiredFiles(CancellationToken cancellationToken);
    }
}
