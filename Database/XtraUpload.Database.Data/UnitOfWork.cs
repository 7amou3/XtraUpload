using System;
using System.Threading.Tasks;
using XtraUpload.Database.Data.Common;

namespace XtraUpload.Database.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        readonly ApplicationDbContext _context;

        public UnitOfWork(ApplicationDbContext context, IUserRepository users, IRoleRepository roles, IRoleClaimsRepository roleClaims, IFileRepository files, IFolderRepository folders,
            IDownloadRepository downloads, IConfirmationKeyRepository confirmationKeys, IFileExtensionRepository fileExtensions, IPageRepository pages)
        {
            _context = context;
            Users = users;
            Roles = roles;
            RoleClaims = roleClaims;
            Files = files;
            Folders = folders;
            Downloads = downloads;
            ConfirmationKeys = confirmationKeys;
            FileExtensions = fileExtensions;
            Pages = pages;
        }

        public IUserRepository Users { get; private set; }
        public IRoleRepository Roles { get; private set; }
        public IRoleClaimsRepository RoleClaims { get; private set; }
        public IFileRepository Files { get; private set; }
        public IFolderRepository Folders { get; private set; }
        public IDownloadRepository Downloads { get; private set; }
        public IConfirmationKeyRepository ConfirmationKeys { get; private set; }
        public IFileExtensionRepository FileExtensions { get; private set; }
        public IPageRepository Pages { get; private set; }

        public async Task<int> CompleteAsync()
        {
            try
            {
                return await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        #region IDisposable support
        private bool disposedValue = false; // To detect redundant calls

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _context.Dispose();
                }

                disposedValue = true;
            }
        }

        #endregion
    }
}
