using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XtraUpload.Domain;
using XtraUpload.WebApp.Common;

namespace XtraUpload.FileManager.Service.Common
{
    public interface IFileManagerService
    {
        Task<DeleteFileResult> DeleteFile(string fileId);
        Task<DeleteFolderResult> DeleteFolder(string folderid);
        Task<DeleteItemsResult> DeleteItems(DeleteItemsViewModel items);
        Task<MoveItemsResult> MoveItems(MoveItemsViewModel items);
        Task<AvatarResult> GetUserAvatar();
    }
}
