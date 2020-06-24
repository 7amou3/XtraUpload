using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using XtraUpload.Database.Data.Common;
using XtraUpload.Domain;
using XtraUpload.Domain.Infra;
using XtraUpload.FileManager.Service.Common;
using XtraUpload.ServerApp.Common;

namespace XtraUpload.FileManager.Service
{
    public class FileManagerService : IFileManagerService
    {
        readonly IUnitOfWork _unitOfWork;
        readonly ClaimsPrincipal _caller;
        readonly UploadOptions _uploadOpt;
        readonly ILogger<FileManagerService> _logger;
        
        public FileManagerService(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, 
            IOptionsMonitor<UploadOptions> uploadOpt, ILogger<FileManagerService> logger)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _uploadOpt = uploadOpt.CurrentValue;
            _caller = httpContextAccessor.HttpContext.User;
        }
        
        #region IFileManagerService members

        /// <summary>
        /// Create a new folder
        /// </summary>
        public async Task<CreateFolderResult> CreateFolder(CreateFolderViewModel folder)
        {
            string userId = _caller.GetUserId();

            FolderItem newFolder = new FolderItem()
            {
                Id = Helpers.GenerateUniqueId(),
                Name = folder.FolderName,
                CreatedAt = DateTime.Now,
                IsAvailableOnline = true,
                LastModified = DateTime.Now,
                Parentid = folder.ParentFolder.Id,
                UserId = userId
            };
            _unitOfWork.Folders.Add(newFolder);

            CreateFolderResult Result = new CreateFolderResult();

            try
            {
                await _unitOfWork.CompleteAsync();
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
                Result.ErrorContent = new ErrorContent("Unknown error occured, please try again", ErrorOrigin.Server);
            }

            Result.Folder = newFolder;
            return Result;
        }

        /// <summary>
        /// Get all user folders
        /// </summary>
        public async Task<GetFoldersResult> GetUserFolders()
        {
            GetFoldersResult Result = new GetFoldersResult();

            string userId = _caller.GetUserId();
            Result.Folders = await _unitOfWork.Folders.FindAsync(s => s.UserId == userId);           
           
            return Result;
        }

        /// <summary>
        /// Get folder(s) relative to it's parent id
        /// </summary>
        public async Task<GetFoldersResult> GetFolders(string parentId)
        {
            GetFoldersResult Result = new GetFoldersResult();
            string userId = _caller.GetUserId();

            FolderItem folder = await _unitOfWork.Folders.FirstOrDefaultAsync(s => s.Id == parentId);
            if (folder == null)
            {
                Result.ErrorContent = new ErrorContent("No folder with the provided id was found", ErrorOrigin.Client);
                return Result;
            }
            // If anonymous user, check if folder is public
            if (userId != folder.UserId && folder.IsAvailableOnline == false)
            {
                Result.ErrorContent = new ErrorContent("This folder is not available for public downloads", ErrorOrigin.Client);
                return Result;
            }

            Result.Folders = await GetFolderChildren(folder, folder.UserId);

            return Result;
        }

        /// <summary>
        /// Get a user folder content
        /// </summary>
        public async Task<GetFolderContentResult> GetUserFolder(string folderid)
        {
            string userId = _caller.GetUserId();   
            // user can pass null as root folder
            string parentid = folderid ?? "root";

            GetFolderContentResult Result = new GetFolderContentResult()
            {
                // Get folders
                Folders = await _unitOfWork.Folders.FindAsync(s => s.Parentid == parentid && s.UserId == userId),
                // get Files, the root folder is represented by a null value in TFile table
                Files = await _unitOfWork.Files.FindAsync(s => s.FolderId == folderid && s.UserId == userId)
            };
           
            return Result;
        }

        /// <summary>
        /// Get a public folder content
        /// </summary>
        public async Task<GetFolderContentResult> GetPublicFolder(PublicFolderViewModel model)
        {
            GetFolderContentResult Result = new GetFolderContentResult();
            string userId = _caller.GetUserId();
            // Display child folder if requested
            string folderId = model.ChildFolderId ?? model.MainFolderId;
            FolderItem folder = await _unitOfWork.Folders.FirstOrDefaultAsync(f => f.Id == folderId);
            // Check if folder exist
            if (folder == null)
            {
                Result.ErrorContent = new ErrorContent("No folder with the provided id was found", ErrorOrigin.Client);
                return Result;
            }
            // If anonymous user, check if folder is public
            if (userId != folder.UserId && folder.IsAvailableOnline == false)
            {
                Result.ErrorContent = new ErrorContent("This folder is not available for public downloads", ErrorOrigin.Client);
                return Result;
            }

            // Get folders
            Result.Folders = await _unitOfWork.Folders.FindAsync(s => s.Parentid == folderId);
            // get Files, the root folder is represented by a null value in TFile table
            Result.Files = await _unitOfWork.Files.FindAsync(s => s.FolderId == folderId);

            return Result;
        }

        /// <summary>
        /// Get a file by it's tus id
        /// </summary>
        public async Task<GetFileResult> GetFileByTusId(string tusid)
        {
            string userId = _caller.GetUserId();

            GetFileResult Result = new GetFileResult()
            {
                File = await _unitOfWork.Files.FirstOrDefaultAsync(s => s.UserId == userId && s.TusId == tusid)
            };
            // Check if file exist
            if (Result.File == null)
            {
                Result.ErrorContent = new ErrorContent("No file with the provided id was found", ErrorOrigin.Client);
            }

            return Result;
        }

        /// <summary>
        /// Get a file by it's id
        /// </summary>
        public async Task<GetFileResult> GetFileById(string fileid)
        {
            GetFileResult Result = new GetFileResult()
            {
                File = await _unitOfWork.Files.FirstOrDefaultAsync(s => s.Id == fileid)
            };

            // Check if file exist
            if (Result.File == null)
            {
                Result.ErrorContent = new ErrorContent("No file with the provided id was found", ErrorOrigin.Client);
            }

            return Result;
        }

        /// <summary>
        /// Updates the online availability state of the given file
        /// </summary>
        public async Task<FileAvailabilityResult> UpdateFileAvailability(string fileId, bool isOnline)
        {
            FileAvailabilityResult Result = new FileAvailabilityResult();

            string userId = _caller.GetUserId();
            FileItem file = await _unitOfWork.Files.FirstOrDefaultAsync(s => s.Id == fileId && s.UserId == userId);

            // Check file exist
            if (file == null)
            {
                Result.ErrorContent = new ErrorContent("No file with the provided id was found", ErrorOrigin.Client);
                return Result;
            }

            // Prepare data
            file.IsAvailableOnline = isOnline;
            file.LastModified = DateTime.Now;

            // Try to save in db
            try
            {
                await _unitOfWork.CompleteAsync();
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
                Result.ErrorContent = new ErrorContent("Unknown error occured, please try again", ErrorOrigin.Server);
            }

            Result.File = file;
            return Result;
        }

        /// <summary>
        /// Update online folder availability
        /// </summary>
        public async Task<FolderAvailabilityResult> UpdateFolderAvailability(string folderId, bool isOnline)
        {
            FolderAvailabilityResult Result = new FolderAvailabilityResult();

            string userId = _caller.Claims.FirstOrDefault(c => c.Type == "id")?.Value;

            var folder = await _unitOfWork.Folders.FirstOrDefaultAsync(s => s.Id == folderId && s.UserId == userId);
            // Check if folder exist
            if (folder == null)
            {
                Result.ErrorContent = new ErrorContent("No folder with the provided id was found", ErrorOrigin.Client);
                return Result;
            }

            // Prepare data
            folder.IsAvailableOnline = isOnline;
            folder.LastModified = DateTime.Now;

            // Try to save in db
            try
            {
                await _unitOfWork.CompleteAsync();
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
                Result.ErrorContent = new ErrorContent("Unknown error occured, please try again", ErrorOrigin.Server);
            }

            Result.Folder = folder;
            return Result;
        }

        /// <summary>
        /// Update the file name
        /// </summary>
        public async Task<RenameFileResult> UpdateFileName(string fileId, string newName)
        {
            RenameFileResult Result = new RenameFileResult();
            string userId = _caller.GetUserId();
            FileItem file = await _unitOfWork.Files.FirstOrDefaultAsync(s => s.Id == fileId && s.UserId == userId);
            // Check if file exist
            if (file == null)
            {
                Result.ErrorContent = new ErrorContent("No file with the provided id was found", ErrorOrigin.Client);
                return Result;
            }
            
            // Prepare data
            file.Name = newName;
            file.LastModified = DateTime.Now;

            // Try to save to db
            try
            {
                await _unitOfWork.CompleteAsync();
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
                Result.ErrorContent = new ErrorContent("Unknown error occured, please try again", ErrorOrigin.Server);
            }

            Result.File = file;
            return Result;
        }

        /// <summary>
        /// Update folder name
        /// </summary>
        public async Task<RenameFolderResult> UpdateFolderName(string folderId, string newName)
        {
            RenameFolderResult Result = new RenameFolderResult();
            string userId = _caller.Claims.Single(c => c.Type == "id")?.Value;
            FolderItem folder = await _unitOfWork.Folders.FirstOrDefaultAsync(s => s.Id == folderId && s.UserId == userId);
            // Check if folder exist
            if (folder == null)
            {
                Result.ErrorContent = new ErrorContent("No folder with the provided id was found", ErrorOrigin.Client);
                return Result;
            }

            // Prepare data
            folder.Name = newName;
            folder.LastModified = DateTime.Now;

            // Try to save to db
            try
            {
                await _unitOfWork.CompleteAsync();
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
                Result.ErrorContent = new ErrorContent("Unknown error occured, please try again", ErrorOrigin.Server);
            }

            Result.Folder = folder;
            return Result;
        }

        /// <summary>
        /// Delete the file from the drive and db
        /// </summary>
        public async Task<DeleteFileResult> DeleteFile(string fileId)
        {
            DeleteFileResult Result = new DeleteFileResult();
            FileItem file = await _unitOfWork.Files.FirstOrDefaultAsync(s => s.Id == fileId);

            // Check if file exist
            if (file == null)
            {
                Result.ErrorContent = new ErrorContent("No file with the provided id was found", ErrorOrigin.Client);
                return Result;
            }

            // Remove file from collection
            _unitOfWork.Files.Remove(file);

            try
            {
                // Delete from db
                await _unitOfWork.CompleteAsync();

                // Delete from disk (no need to queue to background thread, because Directory.Delete does not block)
                string folderPath = Path.Combine(_uploadOpt.UploadPath, file.UserId, file.Id);
                if (Directory.Exists(folderPath))
                {
                    Directory.Delete(folderPath, true);
                }  
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
                Result.ErrorContent = new ErrorContent("Unknown error occured, please try again", ErrorOrigin.Server);
            }

            Result.File = file;
            return Result;
        }

        /// <summary>
        /// Delete a folder and all it's content
        /// </summary>
        public async Task<DeleteFolderResult> DeleteFolder(string folderid)
        {
            DeleteFolderResult Result = new DeleteFolderResult();

            string userId = _caller.GetUserId();
            FolderItem folder = await _unitOfWork.Folders.FirstOrDefaultAsync(s => s.Id == folderid);

            // Check if file exist
            if (folder == null)
            {
                Result.ErrorContent = new ErrorContent("No folder with the provided id was found", ErrorOrigin.Client);
                return Result;
            }

            IEnumerable<FolderItem> folders = await GetFolderChildren(folder, userId);

            // Extract folders id 
            List<string> foldersId = new List<string>() { folder.Id };
            foldersId.AddRange(folders.Select(s => s.Id).ToList());

            // Get all files in those folders
            var files = await _unitOfWork.Files.FindAsync(s => s.UserId == userId && foldersId.Contains(s.FolderId));

            // Delete files from db
            _unitOfWork.Folders.Remove(folder);
            _unitOfWork.Folders.RemoveRange(folders);
            _unitOfWork.Files.RemoveRange(files);

            try
            {
                // Persist changes
                await _unitOfWork.CompleteAsync();

                // Remove the files from the drive (no need to queue to background thread, because Directory.Delete does not block)
                foreach (var file in files)
                {
                    string folderPath = Path.Combine(_uploadOpt.UploadPath, file.UserId, file.Id);

                    if (Directory.Exists(folderPath))
                    {
                        Directory.Delete(folderPath, true);
                    }
                }
            }
            catch (Exception _ex)
            {
                Result.ErrorContent = new ErrorContent(_ex.Message, ErrorOrigin.Server);
                _logger.LogError(Result.ErrorContent.ToString());
            }

            Result.Folders = folders;
            Result.Files = files;
            return Result;
        }

        /// <summary>
        /// Delete folders and files
        /// </summary>
        public async Task<DeleteItemsResult> DeleteItems(DeleteItemsViewModel items)
        {
            DeleteItemsResult result = new DeleteItemsResult();
            IEnumerable<FileItem> files = new List<FileItem>();
            List<DeleteFolderResult> folders = new List<DeleteFolderResult>();

            string userId = _caller.GetUserId();
            // Delete files
            if (items.SelectedFiles.Any())
            {
                List<string> filesIds = items.SelectedFiles.Select(s => s.Id).ToList();
                files = await _unitOfWork.Files.FindAsync(s => filesIds.Contains(s.Id) && s.UserId == userId);
                _unitOfWork.Files.RemoveRange(files);
            }
            // Delete folders
            if (items.SelectedFolders.Any())
            {
                foreach (var folder in items.SelectedFolders)
                {
                    folders.Add(await DeleteFolder(folder.Id));
                }
            }

            try
            {
                // Persist changes
                await _unitOfWork.CompleteAsync();

                // Remove the files from the drive (no need to queue to background thread, because Directory.Delete does not block)
                foreach (var file in files)
                {
                    string folderPath = Path.Combine(_uploadOpt.UploadPath, file.UserId, file.Id);

                    if (Directory.Exists(folderPath))
                    {
                        Directory.Delete(folderPath, true);
                    }
                }
            }
            catch (Exception _ex)
            {
                result.ErrorContent = new ErrorContent(_ex.Message, ErrorOrigin.Server);
                _logger.LogError(result.ErrorContent.ToString());
            }

            result.Folders = folders;
            result.Files = files;
            return result;
        }

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
            try
            {
                await _unitOfWork.CompleteAsync();
                
            }
            catch (Exception _ex)
            {
                _logger.LogError(_ex.Message);
                Result.ErrorContent = new ErrorContent("Unknown error occured, please try again", ErrorOrigin.Server);
            }

            Result.MovedItemsIds = movedIds;
            // Get the new folder structure
            Result.Folders = await _unitOfWork.Folders.FindAsync(f => f.UserId == userId);
            return Result;
        }
        #endregion
    }
}
