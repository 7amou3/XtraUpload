using System;
using System.Collections.Generic;
using System.Text;
using XtraUpload.Database.Data.Common;
using XtraUpload.Domain;

namespace XtraUpload.Database.Data
{
    public class FileExtensionRepository : Repository<FileExtension>, IFileExtensionRepository
    {
        readonly ApplicationDbContext _context;

        public FileExtensionRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _context = dbContext;
        }
    }
}
