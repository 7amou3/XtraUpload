using System;
using System.Threading.Tasks;
using XtraUpload.Domain;

namespace XtraUpload.Database.Data.Common
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        IRoleRepository Roles { get; }
        IRoleClaimsRepository RoleClaims { get; }
        IFileRepository Files { get; }
        IFolderRepository Folders { get; }
        IDownloadRepository Downloads { get; }
        IConfirmationKeyRepository ConfirmationKeys { get; }
        IFileExtensionRepository FileExtensions { get; }
        IPageRepository Pages { get; }
        IStorageServerRepository StorageServer { get; }
        ILanguageRepository Languages { get; }

        Task<int> CompleteAsync();
        Task<T> CompleteAsync<T>(T result) where T : OperationResult;
    }
}
