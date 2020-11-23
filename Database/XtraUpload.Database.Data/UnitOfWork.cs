using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using XtraUpload.Database.Data.Common;
using XtraUpload.Domain;

namespace XtraUpload.Database.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        readonly ApplicationDbContext _context;
        readonly ILogger<UnitOfWork> _logger;

        public UnitOfWork(
            ApplicationDbContext context, 
            IUserRepository users,
            IRoleRepository roles, 
            IRoleClaimsRepository roleClaims, 
            IFileRepository files, 
            IFolderRepository folders,
            IDownloadRepository downloads, 
            IConfirmationKeyRepository confirmationKeys,
            IFileExtensionRepository fileExtensions, 
            IPageRepository pages,
            IStorageServerRepository storageServer,
            ILanguageRepository languages,
            ILogger<UnitOfWork> logger)
        {
            _context = context;
            _logger = logger;
            Users = users;
            Roles = roles;
            RoleClaims = roleClaims;
            Files = files;
            Folders = folders;
            Downloads = downloads;
            ConfirmationKeys = confirmationKeys;
            FileExtensions = fileExtensions;
            Pages = pages;
            StorageServer = storageServer;
            Languages = languages;
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
        public IStorageServerRepository StorageServer { get; private set; }
        public ILanguageRepository Languages { get; private set; }

        public async Task<int> CompleteAsync()
        {
            int result;
            try
            {
                result = await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                result = -1;
            }

            return result;
        }
        public async Task<T> CompleteAsync<T>(T result) where T: OperationResult
        {
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                result.ErrorContent = new ErrorContent("Unknown error occured, please try again", ErrorOrigin.Server);
                _logger.LogError(ex.Message.ToString());
            }
            
            return result;
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
