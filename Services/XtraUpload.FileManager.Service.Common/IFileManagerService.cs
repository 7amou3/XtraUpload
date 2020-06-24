using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XtraUpload.Domain;
using XtraUpload.ServerApp.Common;

namespace XtraUpload.FileManager.Service.Common
{
    public interface IFileManagerService
    {
        Task<GetFileResult> GetFileByTusId(string tusid);
        Task<GetFileResult> GetFileById(string fileid);
        Task<GetFolderContentResult> GetUserFolder(string folderId);
        Task<GetFolderContentResult> GetPublicFolder(PublicFolderViewModel folderid);
        Task<GetFoldersResult> GetUserFolders();
        Task<GetFoldersResult> GetFolders(string parentId);
        Task<CreateFolderResult> CreateFolder(CreateFolderViewModel folder);
        Task<FileAvailabilityResult> UpdateFileAvailability(string fileId, bool isOnline);
        Task<FolderAvailabilityResult> UpdateFolderAvailability(string folderId, bool isOnline);
        Task<RenameFileResult> UpdateFileName(string fileId, string newName);
        Task<RenameFolderResult> UpdateFolderName(string folderId, string newName);
        Task<DeleteFileResult> DeleteFile(string fileId);
        Task<DeleteFolderResult> DeleteFolder(string folderid);
        Task<DeleteItemsResult> DeleteItems(DeleteItemsViewModel items);
        Task<MoveItemsResult> MoveItems(MoveItemsViewModel items);
    }
}
