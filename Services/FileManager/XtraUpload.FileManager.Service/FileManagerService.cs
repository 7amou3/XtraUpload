using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using XtraUpload.Database.Data.Common;
using XtraUpload.Domain;
using XtraUpload.Domain.Infra;
using XtraUpload.FileManager.Service.Common;
using XtraUpload.WebApp.Common;

namespace XtraUpload.FileManager.Service
{
    public class FileManagerService : IFileManagerService
    {
        readonly IUnitOfWork _unitOfWork;
        readonly ClaimsPrincipal _caller;
        readonly UploadOptions _uploadOpt;
        
        public FileManagerService(
            IUnitOfWork unitOfWork, 
            IHttpContextAccessor httpContextAccessor, 
            IOptionsMonitor<UploadOptions> uploadOpt)
        {
            _unitOfWork = unitOfWork;
            _uploadOpt = uploadOpt.CurrentValue;
            _caller = httpContextAccessor.HttpContext.User;
        }
        
        #region IFileManagerService members

        /// <summary>
        /// Recursively get all folders within a folder
        /// </summary>
        private async Task<IEnumerable<FolderItem>> GetFolderChildren(FolderItem folder, string userId)
        {
            IEnumerable<FolderItem> userFolders = await _unitOfWork.Folders.FindAsync(s => s.UserId == userId);

            List<FolderItem> childFolders = new List<FolderItem>();

            void _getChildFolders(string id)
            {
                var childs = userFolders.Where(s => s.Parentid == id).ToList();
                childFolders.AddRange(childs);
                // Recursively get child folder
                childs.ForEach(c => _getChildFolders(c.Id));
            }
            _getChildFolders(folder.Id);

            childFolders.Add(folder);
            return childFolders;
        }

        /// <summary>
        /// Move the selected items to the specified folder
        /// </summary>
        public async Task<MoveItemsResult> MoveItems(MoveItemsViewModel items)
        {
            MoveItemsResult Result = new MoveItemsResult();

            List<string> movedIds = new List<string>();
            string userId = _caller.GetUserId();

            FolderItem destFolder = await _unitOfWork.Folders.FirstOrDefaultAsync(s => s.Id == items.DestFolderId);
            // Check destination folder exists
            if (destFolder == null && items.DestFolderId != "root")
            {
                Result.ErrorContent = new ErrorContent("No folder with the provided id was found", ErrorOrigin.Client);
                return Result;
            }
            
            if (items.SelectedFolders.Any())
            {
                // Check if user not trying to move folders to same location
                if (items.SelectedFolders.Any(s => s.Parentid == items.DestFolderId || s.Id == items.DestFolderId))
                {
                    Result.ErrorContent = new ErrorContent("Moving folders to the same location is not possible!", ErrorOrigin.Client);
                    return Result;
                }
                // Get the child folders of the currently moved folder(s)
                List<FolderItem> childFolders = new List<FolderItem>();
                foreach (FolderItem f in items.SelectedFolders)
                {
                    childFolders.AddRange(await GetFolderChildren(f, userId));
                }
                childFolders.RemoveAt(childFolders.Count - 1);
                if (childFolders.Any(s => s.Id == items.DestFolderId))
                {
                    Result.ErrorContent = new ErrorContent("Moving a parent folder to one of its child folders is not possible", ErrorOrigin.Client);
                    return Result;
                }

                var foldersId = items.SelectedFolders.Select(s => s.Id);
                var folders = await _unitOfWork.Folders.FindAsync(s => foldersId.Contains(s.Id));
                foreach (FolderItem folder in folders)
                {
                    folder.Parentid = items.DestFolderId;
                    movedIds.Add(folder.Id);
                }
            }
            if (items.SelectedFiles.Any())
            {
                // Check if user not trying to move files to same location
                if (items.SelectedFiles.Any(s => s.FolderId == items.DestFolderId || (s.FolderId == null && items.DestFolderId == "root")))
                {
                    Result.ErrorContent = new ErrorContent("Moving files to the same location is not possible!", ErrorOrigin.Client);
                    return Result;
                }
                IEnumerable<string> filesId = items.SelectedFiles.Select(s => s.Id);
                IEnumerable<FileItem> files = await _unitOfWork.Files.FindAsync(s => filesId.Contains(s.Id));

                // Update file's folder id
                foreach (FileItem file in files)
                {
                    file.FolderId = items.DestFolderId == "root" ? null : items.DestFolderId;
                    movedIds.Add(file.Id);
                }
            }

            // Try to save to db
            Result = await _unitOfWork.CompleteAsync(Result);
            if (Result.State == OperationState.Success)
            {
                Result.MovedItemsIds = movedIds;
                // Get the new folder structure
                Result.Folders = await _unitOfWork.Folders.FindAsync(f => f.UserId == userId);
            }

            return Result;
        }

        /// <summary>
        /// Get user's avatar url
        /// </summary>
        /// <returns></returns>
        public async Task<AvatarResult> GetUserAvatar()
        {
            AvatarResult result = new AvatarResult();
            string userId = _caller.GetUserId();
            var user = await _unitOfWork.Users.FirstOrDefaultAsync(s => s.Id == userId);
            if (user == null)
            {
                result.ErrorContent = new ErrorContent("No user with the provided id was found", ErrorOrigin.Client);
                return result;
            }
            result.Url = user.Avatar;
            return result;
        }
        #endregion
    }
}
