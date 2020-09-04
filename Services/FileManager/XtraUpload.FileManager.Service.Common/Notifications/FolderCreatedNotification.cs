using MediatR;
using XtraUpload.Domain;

namespace XtraUpload.FileManager.Service.Common
{
    /// <summary>
    /// Notifcation raised when a folder is created
    /// </summary>
    public class FolderCreatedNotification: INotification
    {
        public FolderCreatedNotification(FolderItem folder)
        {
            Folder = folder;
        }
        public FolderItem Folder { get; }
    }
}
