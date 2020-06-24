using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using XtraUpload.Administration.Service.Common;
using XtraUpload.Authentication.Service.Common;
using XtraUpload.Database.Data.Common;
using XtraUpload.Domain;
using XtraUpload.Domain.Infra;
using XtraUpload.ServerApp.Common;

namespace XtraUpload.Administration.Service
{
    public class AdministrationService : IAdministrationService
    {
        readonly IUnitOfWork _unitOfWork;
        readonly UploadOptions _uploadOpts;
        readonly ILogger<AdministrationService> _logger;

        public AdministrationService(IUnitOfWork unitOfWork, IOptionsMonitor<UploadOptions> uploadsOpts, ILogger<AdministrationService> logger)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _uploadOpts = uploadsOpts.CurrentValue;
        }

        readonly Dictionary<FileType, List<string>> fileTypes = new Dictionary<FileType, List<string>>()
        {
            { FileType.Archives, new List<string>() {".rar", ".zip", ".tar", ".gzip", ".aaf", ".iso", ".bin" } },
            { FileType.Multimedia, new List<string>() {".mp4", ".flv", ".mov", ".avi", ".mp3", ".wav", ".png", ".gif", ".jpe" ,".jpg", ".jpeg"} },
            { FileType.Documents, new List<string>() {".docx", ".pdf", ".txt", ".xml", ".xlsx", ".csv", ".pptx"} }
        };

        public async Task<AdminOverViewResult> AdminOverView(DateRangeViewModel range)
        {
            AdminOverViewResult Result = new AdminOverViewResult();
            // Check date range is valid
            if (range.Start.Date >= range.End.Date)
            {
                Result.ErrorContent = new ErrorContent("Invalid date range.", ErrorOrigin.Client);
                return Result;
            }

            long freeSpace = 0;
            long totalsize = 0;
            string rootDrive = Path.GetPathRoot(_uploadOpts.UploadPath);
            DriveInfo driveInfo = DriveInfo.GetDrives().FirstOrDefault(s => s.Name == rootDrive);
            if (driveInfo != null)
            {
                freeSpace = driveInfo.TotalFreeSpace;
                totalsize = driveInfo.TotalSize;
            }
            else
            {
                #region Trace
                _logger.LogError($"No drive found with the name: {rootDrive}, please check your appsettings.json configs");
                #endregion
            }

            Result.TotalFiles = await _unitOfWork.Files.CountAsync();
            Result.TotalUsers = await _unitOfWork.Users.CountAsync();
            Result.FilesCount = await GetUploadsHistory(range);
            Result.UsersCount = await GetUsersHistory(range);
            Result.FileTypesCount = await GetFileTypesCount(range);
            Result.DriveSize = totalsize;
            Result.FreeSpace = freeSpace;

            return Result;
        }

        public async Task<AdminOverViewResult> UploadCounts(DateRangeViewModel range)
        {
            AdminOverViewResult Result = new AdminOverViewResult();
            // Check date range is valid
            if (range.Start.Date > range.End.Date)
            {
                Result.ErrorContent = new ErrorContent("Invalid date range.", ErrorOrigin.Client);
                return Result;
            }

            Result.FilesCount = await GetUploadsHistory(range);
            return Result;
        }

        public async Task<AdminOverViewResult> UserCounts(DateRangeViewModel range)
        {
            AdminOverViewResult Result = new AdminOverViewResult();
            // Check date range is valid
            if (range.Start.Date > range.End.Date)
            {
                Result.ErrorContent = new ErrorContent("Invalid date range.", ErrorOrigin.Client);
                return Result;
            }

            Result.UsersCount = await GetUsersHistory(range);
            return Result;
        }

        /// <summary>
        /// Get file type count grouped by the given period of time
        /// </summary>
        public async Task<AdminOverViewResult> FileTypesCounts(DateRangeViewModel range)
        {
            AdminOverViewResult Result = new AdminOverViewResult();
            // Check date range is valid
            if (range.Start.Date > range.End.Date)
            {
                Result.ErrorContent = new ErrorContent("Invalid date range.", ErrorOrigin.Client);
                return Result;
            }

            Result.FileTypesCount = await GetFileTypesCount(range);
            return Result;
        }

        /// <summary>
        /// Get a list of users
        /// </summary>
        public async Task<PagingResult<UserExtended>> GetUsers(PageSearchViewModel model)
        {
            PagingResult<UserExtended> result = new PagingResult<UserExtended>();
            Expression<Func<User, bool>> criteria = s => true;

            if (model.Start != null && model.End != null)
            {
                criteria = criteria.And(s => s.CreatedAt > model.Start && s.CreatedAt < model.End);
            }
            if (model.UserId != null && model.UserId != Guid.Empty)
            {
                criteria = criteria.And(s => s.Id == model.UserId.ToString());
            }

            result.TotalItems = await _unitOfWork.Users.CountAsync(criteria);
            result.Items = await _unitOfWork.Users.GetUsers(model, criteria);

            return result;
        }

        /// <summary>
        /// Edit a user
        /// </summary>
        public async Task<EditUserResult> EditUser(EditUserViewModel model)
        {
            EditUserResult result = new EditUserResult();
            // Check email is not duplicated
            User duplicatedUser = await _unitOfWork.Users.FirstOrDefaultAsync(s => s.Email == model.Email && s.Id != model.Id);
            if (duplicatedUser != null)
            {
                result.ErrorContent = new ErrorContent("A user with the provided email already exists.", ErrorOrigin.Client);
                return result;
            }
            // Check user exists
            User user = await _unitOfWork.Users.FirstOrDefaultAsync(s => s.Id == model.Id);
            if (user == null)
            {
                result.ErrorContent = new ErrorContent("No user found with provided Id.", ErrorOrigin.Client);
                return result;
            }
            
            if (!string.IsNullOrEmpty(model.NewPassword))
            {
                user.Password = Helpers.HashPassword(model.NewPassword);
            }
            user.Email = model.Email;
            user.RoleId = model.RoleId;
            user.UserName = model.UserName;
            user.EmailConfirmed = model.EmailConfirmed;
            user.AccountSuspended = model.SuspendAccount;
            user.LastModified = DateTime.Now;

            // Persist changes
            try
            {
                await _unitOfWork.CompleteAsync();
            }
            catch (Exception _ex)
            {
                result.ErrorContent = new ErrorContent(_ex.Message, ErrorOrigin.Server);
                _logger.LogError(result.ErrorContent.ToString());
            }
            return result;
        }

        /// <summary>
        /// Delete a user
        /// </summary>
        public async Task<OperationResult> DeleteUsers(IEnumerable<string> usersId)
        {
            OperationResult result = new OperationResult();
            IEnumerable<User> users = await _unitOfWork.Users.FindAsync(s => usersId.Contains(s.Id));
            // Check if users exist
            if (!users.Any())
            {
                result.ErrorContent = new ErrorContent("No user found whith the provided Ids.", ErrorOrigin.Client);
                return result;
            }
            // Get users files
            IEnumerable<FileItem> files = await _unitOfWork.Files.FindAsync(s => usersId.Contains(s.UserId));

            // Delete from db
            _unitOfWork.Users.RemoveRange(users);
            _unitOfWork.Files.RemoveRange(files);

            // Persist changes
            try
            {
                await _unitOfWork.CompleteAsync();
                // Remove the files from the drive (no need to queue to background thread, because Directory.Delete does not block)
                foreach (var file in files)
                {
                    string folderPath = Path.Combine(_uploadOpts.UploadPath, file.UserId, file.Id);

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
            return result;
        }

        /// <summary>
        /// Get a list of files 
        /// </summary>
        public async Task<PagingResult<FileItemExtended>> GetFiles(PageSearchViewModel model)
        {
            PagingResult<FileItemExtended> Result = new PagingResult<FileItemExtended>();

            Expression<Func<FileItem, bool>> criteria = s => true;

            if (model.Start != null && model.End != null)
            {
                criteria = criteria.And(s => s.CreatedAt > model.Start && s.CreatedAt < model.End);
            }
            if (model.UserId != null && model.UserId != Guid.Empty)
            {
                criteria = criteria.And(s => s.UserId == model.UserId.ToString());
            }
            if (model.FileExtension != null)
            {
                criteria = criteria.And(s => s.Extension == model.FileExtension);
            }

            Result.TotalItems = await _unitOfWork.Files.CountAsync(criteria);
            Result.Items = await _unitOfWork.Files.GetFiles(model, criteria);

            return Result;
        }

        /// <summary>
        /// Get a list of file extensions
        /// </summary>
        public async Task<FileExtensionsResult> GetFileExtensions()
        {
            var Result = new FileExtensionsResult()
            {
                FileExtensions = await _unitOfWork.FileExtensions.GetAll()
            };
            return Result;
        }

        /// <summary>
        /// Search for users by name
        /// </summary>
        public async Task<SearchUserResult> SearchUsers(string name)
        {
            SearchUserResult result = new SearchUserResult()
            {
                Users = await _unitOfWork.Users.SearchUsersByName(name)
            };

            return result;
        }

        /// <summary>
        /// Add an extension
        /// </summary>
        public async Task<FileExtensionResult> AddExtension(string name)
        {
            FileExtensionResult result = new FileExtensionResult();
            FileExtension newFileType = new FileExtension()
            {
                Name = name
            };
            _unitOfWork.FileExtensions.Add(newFileType);

            // Save to db
            try
            {
                await _unitOfWork.CompleteAsync();
            }
            catch (Exception _ex)
            {
                result.ErrorContent = new ErrorContent("Unknown error occured, please try again", ErrorOrigin.Server);
                _logger.LogError(_ex.Message);
            }

            result.FileExtension = newFileType;
            return result;
        }

        /// <summary>
        /// Update extension name
        /// </summary>
        public async Task<FileExtensionResult> UpdateExtension(EditExtensionViewModel model)
        {
            FileExtensionResult result = new FileExtensionResult();
            FileExtension ext = await _unitOfWork.FileExtensions.FirstOrDefaultAsync(s => s.Id == model.Id);
            if (ext == null)
            {
                result.ErrorContent = new ErrorContent("No file type found with the provided id.", ErrorOrigin.Client);
                return result;
            }

            // update extension name
            ext.Name = model.NewExt;

            // Save to db
            try
            {
                await _unitOfWork.CompleteAsync();
            }
            catch (Exception _ex)
            {
                result.ErrorContent = new ErrorContent("Unknown error occured, please try again", ErrorOrigin.Server);
                _logger.LogError(_ex.Message);
            }

            return result;
        }

        /// <summary>
        /// Delete extension
        /// </summary>
        public async Task<OperationResult> DeleteExtension(int id)
        {
            OperationResult result = new OperationResult();
            FileExtension ext = await _unitOfWork.FileExtensions.FirstOrDefaultAsync(s => s.Id == id);
            if (ext == null)
            {
                result.ErrorContent = new ErrorContent("No file type found with the provided id.", ErrorOrigin.Client);
                return result;
            }

            // remove from collection
            _unitOfWork.FileExtensions.Remove(ext);

            // Save to db
            try
            {
                await _unitOfWork.CompleteAsync();
            }
            catch (Exception _ex)
            {
                result.ErrorContent = new ErrorContent("Unknown error occured, please try again", ErrorOrigin.Server);
                _logger.LogError(_ex.Message);
            }

            return result;
        }

        /// <summary>
        /// Delete file(s) by id
        /// </summary>
        public async Task<DeleteFilesResult> DeleteFiles(IEnumerable<string> ids)
        {
            DeleteFilesResult result = new DeleteFilesResult();
            IEnumerable<FileItem> files = await _unitOfWork.Files.FindAsync(s => ids.Contains(s.Id));
            if (files != null && files.Any())
            {
                // remove from collection
                _unitOfWork.Files.RemoveRange(files);
                // Save to db
                try
                {
                    await _unitOfWork.CompleteAsync();

                    // Delete from disk (no need to queue to background thread, because Directory.Delete does not block)
                    foreach (FileItem file in files)
                    {
                        string folderPath = Path.Combine(_uploadOpts.UploadPath, file.UserId, file.Id);
                        if (Directory.Exists(folderPath))
                        {
                            Directory.Delete(folderPath, true);
                        }
                    }
                }
                catch (Exception _ex)
                {
                    result.ErrorContent = new ErrorContent("Unknown error occured, please try again", ErrorOrigin.Server);
                    _logger.LogError(_ex.Message);
                }
            }
            else
            {
                result.ErrorContent = new ErrorContent($"Please check the provided id(s)", ErrorOrigin.Server);
            }

            result.Files = files;
            return result;
        }

        /// <summary>
        /// Get the users role
        /// </summary>
        public async Task<RolesResult> GetUsersRole()
        {
            RolesResult Result = new RolesResult()
            {
                Roles = await _unitOfWork.Users.GetUsersRoleClaims()
            };
            return Result;
        }
        /// <summary>
        /// Add a new role claims
        /// </summary>
        public async Task<RoleClaimsResult> AddRoleClaims(RoleClaimsViewModel model)
        {
            RoleClaimsResult result = new RoleClaimsResult();
            IEnumerable<RoleClaimsResult> allRoleClaims = await _unitOfWork.Users.GetUsersRoleClaims();
            // Check Role name is not duplicated
            if (allRoleClaims.Any(s => s.Role.Name == model.Role.Name))
            {
                result.ErrorContent = new ErrorContent($"A role with the name {model.Role.Name} already exists", ErrorOrigin.Client);
                return result;
            }

            // Add role
            var role = new Role
            {
                Id = Helpers.GenerateUniqueId(),
                Name = model.Role.Name
            };
            _unitOfWork.Roles.Add(role);
            // Add claims
            List<RoleClaim> claims = new List<RoleClaim>();
            if (model.Claims.AdminAreaAccess != null && model.Claims.AdminAreaAccess.Value)
            {
                claims.Add(new RoleClaim() { RoleId = role.Id, ClaimType = XtraUploadClaims.AdminAreaAccess.ToString(), ClaimValue = "1" });
            }
            if (model.Claims.FileManagerAccess != null && model.Claims.FileManagerAccess.Value)
            {
                claims.Add(new RoleClaim() { RoleId = role.Id, ClaimType = XtraUploadClaims.FileManagerAccess.ToString(), ClaimValue = "1" });
            }
            if (model.Claims.ConcurrentUpload != null)
            {
                claims.Add(new RoleClaim() { RoleId = role.Id, ClaimType = XtraUploadClaims.ConcurrentUpload.ToString(), ClaimValue = model.Claims.ConcurrentUpload.ToString() });
            }
            if (model.Claims.DownloadSpeed != null)
            {
                claims.Add(new RoleClaim() { RoleId = role.Id, ClaimType = XtraUploadClaims.DownloadSpeed.ToString(), ClaimValue = model.Claims.DownloadSpeed.ToString() });
            }
            if (model.Claims.DownloadTTW != null)
            {
                claims.Add(new RoleClaim() { RoleId = role.Id, ClaimType = XtraUploadClaims.DownloadTTW.ToString(), ClaimValue = model.Claims.DownloadTTW.ToString() });
            }
            if (model.Claims.FileExpiration != null)
            {
                claims.Add(new RoleClaim() { RoleId = role.Id, ClaimType = XtraUploadClaims.FileExpiration.ToString(), ClaimValue = model.Claims.FileExpiration.ToString() });
            }
            if (model.Claims.MaxFileSize != null)
            {
                claims.Add(new RoleClaim() { RoleId = role.Id, ClaimType = XtraUploadClaims.MaxFileSize.ToString(), ClaimValue = model.Claims.MaxFileSize.ToString() });
            }
            if (model.Claims.StorageSpace != null)
            {
                claims.Add(new RoleClaim() { RoleId = role.Id, ClaimType = XtraUploadClaims.StorageSpace.ToString(), ClaimValue = model.Claims.StorageSpace.ToString() });
            }
            if (model.Claims.WaitTime != null)
            {
                claims.Add(new RoleClaim() { RoleId = role.Id, ClaimType = XtraUploadClaims.WaitTime.ToString(), ClaimValue = model.Claims.WaitTime.ToString() });
            }

            _unitOfWork.RoleClaims.AddRange(claims);
            // Save to db
            try
            {
                await _unitOfWork.CompleteAsync();
            }
            catch (Exception _ex)
            {
                result.ErrorContent = new ErrorContent("Unknown error occured, please try again", ErrorOrigin.Server);
                _logger.LogError(_ex.Message);
            }

            result.Role = role;
            result.Claims = claims;
            return result;
        }

        /// <summary>
        /// Updates claims of a role
        /// </summary>
        public async Task<RoleClaimsResult> UpdateRoleClaims(RoleClaimsViewModel model)
        {
            RoleClaimsResult result = new RoleClaimsResult();

            IEnumerable<RoleClaimsResult> allRoleClaims = await _unitOfWork.Users.GetUsersRoleClaims();
            IEnumerable<RoleClaim> roleclaims = allRoleClaims.SelectMany(s => s.Claims).Where(s => s.Role.Id != model.Role.Id);
            // Check at least one admin group exist
            if (roleclaims != null
                && !roleclaims.Any(s => s.ClaimType == XtraUploadClaims.AdminAreaAccess.ToString())
                && model.Claims.AdminAreaAccess == false)
            {
                result.ErrorContent = new ErrorContent("At least one Admin group should exist", ErrorOrigin.Client);
                return result;
            }

            Role role = allRoleClaims.SingleOrDefault(s => s.Role.Id == model.Role.Id).Role;
            if (role == null)
            {
                result.ErrorContent = new ErrorContent($"No role found with id {model.Role.Id}", ErrorOrigin.Client);
                return result;
            }
            // Check Role name is not duplicated
            if (allRoleClaims.Any(s => s.Role.Name == role.Name && s.Role.Id != role.Id))
            {
                result.ErrorContent = new ErrorContent($"A role with the name {model.Role.Name} already exists", ErrorOrigin.Client);
                return result;
            }

            // Update Role name
            role.Name = model.Role.Name;

            // Update Role claims
            IEnumerable<RoleClaim> claims = await _unitOfWork.RoleClaims.FindAsync(s => s.RoleId == model.Role.Id);
            _unitOfWork.RoleClaims.RemoveRange(claims);
            List<RoleClaim> updatedClaims = new List<RoleClaim>();

            if (claims.Any())
            {
                if (model.Claims.AdminAreaAccess != null && model.Claims.AdminAreaAccess.Value)
                {
                    updatedClaims.Add(new RoleClaim() { RoleId = model.Role.Id, ClaimType = XtraUploadClaims.AdminAreaAccess.ToString(), ClaimValue = "1" });
                }
                if (model.Claims.FileManagerAccess != null && model.Claims.FileManagerAccess.Value)
                {
                    updatedClaims.Add(new RoleClaim() { RoleId = model.Role.Id, ClaimType = XtraUploadClaims.FileManagerAccess.ToString(), ClaimValue = "1" });
                }
                if (model.Claims.ConcurrentUpload != null)
                {
                    updatedClaims.Add(new RoleClaim() { RoleId = model.Role.Id, ClaimType = XtraUploadClaims.ConcurrentUpload.ToString(), ClaimValue = model.Claims.ConcurrentUpload.ToString() });
                }
                if (model.Claims.DownloadSpeed != null)
                {
                    updatedClaims.Add(new RoleClaim() { RoleId = model.Role.Id, ClaimType = XtraUploadClaims.DownloadSpeed.ToString(), ClaimValue = model.Claims.DownloadSpeed.ToString() });
                }
                if (model.Claims.DownloadTTW != null)
                {
                    updatedClaims.Add(new RoleClaim() { RoleId = model.Role.Id, ClaimType = XtraUploadClaims.DownloadTTW.ToString(), ClaimValue = model.Claims.DownloadTTW.ToString() });
                }
                if (model.Claims.FileExpiration != null)
                {
                    updatedClaims.Add(new RoleClaim() { RoleId = model.Role.Id, ClaimType = XtraUploadClaims.FileExpiration.ToString(), ClaimValue = model.Claims.FileExpiration.ToString() });
                }
                if (model.Claims.MaxFileSize != null)
                {
                    updatedClaims.Add(new RoleClaim() { RoleId = model.Role.Id, ClaimType = XtraUploadClaims.MaxFileSize.ToString(), ClaimValue = model.Claims.MaxFileSize.ToString() });
                }
                if (model.Claims.StorageSpace != null)
                {
                    updatedClaims.Add(new RoleClaim() { RoleId = model.Role.Id, ClaimType = XtraUploadClaims.StorageSpace.ToString(), ClaimValue = model.Claims.StorageSpace.ToString() });
                }
                if (model.Claims.WaitTime != null)
                {
                    updatedClaims.Add(new RoleClaim() { RoleId = model.Role.Id, ClaimType = XtraUploadClaims.WaitTime.ToString(), ClaimValue = model.Claims.WaitTime.ToString() });
                }

                _unitOfWork.RoleClaims.AddRange(updatedClaims);

                // Save to db
                try
                {
                    await _unitOfWork.CompleteAsync();
                }
                catch (Exception _ex)
                {
                    result.ErrorContent = new ErrorContent("Unknown error occured, please try again", ErrorOrigin.Server);
                    _logger.LogError(_ex.Message);
                }
            }

            result.Role = role;
            result.Claims = updatedClaims;
            return result;
        }

        /// <summary>
        /// Delete a role claim
        /// </summary>
        public async Task<OperationResult> DeleteRoleClaims(string roleId)
        {
            OperationResult result = new OperationResult();
            Role role = await _unitOfWork.Roles.FirstOrDefaultAsync(s => s.Id == roleId);
            if (role == null)
            {
                result.ErrorContent = new ErrorContent("No role found with the provided id.", ErrorOrigin.Client);
                return result;
            }
            // Default role can not be deleted
            if (role.IsDefault)
            {
                result.ErrorContent = new ErrorContent("Default role can not be deleted.", ErrorOrigin.Client);
                return result;
            }

            _unitOfWork.Roles.Remove(role);
            // Save to db
            try
            {
                await _unitOfWork.CompleteAsync();
            }
            catch (Exception _ex)
            {
                result.ErrorContent = new ErrorContent("Unknown error occured, please try again", ErrorOrigin.Server);
                _logger.LogError(_ex.Message);
            }

            return result;
        }
        /// <summary>
        /// Get all pages
        /// </summary>
        public async Task<PagesResult> GetPages()
        {
            return new PagesResult()
            {
                Pages = await _unitOfWork.Pages.GetAll()
            };
        }
        /// <summary>
        /// Add a new page
        /// </summary>
        public async Task<PageResult> AddPage(Page p)
        {
            PageResult result = new PageResult();
            // Check page name is unique
            Page pageNameUnique = await _unitOfWork.Pages.FirstOrDefaultAsync(s => s.Name == p.Name);
            if (pageNameUnique != null)
            {
                result.ErrorContent = new ErrorContent($"A page with the same name already exists", ErrorOrigin.Client);
                return result;
            }
            p.Id = Helpers.GenerateUniqueId();
            p.CreatedAt = DateTime.Now;
            p.UpdatedAt = DateTime.Now;
            p.Url = Regex.Replace(p.Name.ToLower(), @"\s+", "_");
            _unitOfWork.Pages.Add(p);

            // Save to db
            try
            {
                await _unitOfWork.CompleteAsync();
            }
            catch (Exception _ex)
            {
                result.ErrorContent = new ErrorContent("Unknown error occured, please try again", ErrorOrigin.Server);
                _logger.LogError(_ex.Message);
            }

            result.Page = p;
            return result;
        }
        /// <summary>
        /// Update a page
        /// </summary>
        public async Task<PageResult> UpdatePage(Page p)
        {
            PageResult result = new PageResult();
            Page page = await _unitOfWork.Pages.FirstOrDefaultAsync(s => s.Id == p.Id);
            // Check page exist
            if (page == null)
            {
                result.ErrorContent = new ErrorContent($"The page {p.Name} does not exist", ErrorOrigin.Client);
                return result;
            }
            // Check page name is unique
            Page pageNameUnique = await _unitOfWork.Pages.FirstOrDefaultAsync(s => s.Name == p.Name && s.Id != p.Id);
            if (pageNameUnique != null)
            {
                result.ErrorContent = new ErrorContent($"A page with the same name already exists", ErrorOrigin.Client);
                return result;
            }
            // Update
            page.UpdatedAt = DateTime.Now;
            page.Content = p.Content;
            page.Name = p.Name;
            page.Url = Regex.Replace(p.Name.ToLower(), @"\s+", "_");

            // Save to db
            try
            {
                await _unitOfWork.CompleteAsync();
            }
            catch (Exception _ex)
            {
                result.ErrorContent = new ErrorContent("Unknown error occured, please try again", ErrorOrigin.Server);
                _logger.LogError(_ex.Message);
            }

            result.Page = page;
            return result;
        }

        /// <summary>
        /// Delete a Page
        /// </summary>
        public async Task<OperationResult> DeletePage(string Id)
        {
            OperationResult result = new OperationResult();
            // Get page name
            Page page = await _unitOfWork.Pages.FirstOrDefaultAsync(s => s.Id == Id);
            if (page == null)
            {
                result.ErrorContent = new ErrorContent($"The requested page was not found", ErrorOrigin.Client);
                return result;
            }

            _unitOfWork.Pages.Remove(page);

            // Save to db
            try
            {
                await _unitOfWork.CompleteAsync();
            }
            catch (Exception _ex)
            {
                result.ErrorContent = new ErrorContent("Unknown error occured, please try again", ErrorOrigin.Server);
                _logger.LogError(_ex.Message);
            }

            return result;
        }

        private async Task<IEnumerable<ItemCountResult>> GetUploadsHistory(DateRangeViewModel range)
        {
            List<ItemCountResult> files = new List<ItemCountResult>(await _unitOfWork.Files.FilesCountByDateRange(range.Start, range.End));

            return FormatResult(range, files);
        }

        private async Task<IEnumerable<ItemCountResult>> GetUsersHistory(DateRangeViewModel range)
        {
            List<ItemCountResult> users = new List<ItemCountResult>(await _unitOfWork.Users.UsersCountByDateRange(range.Start, range.End));

            return FormatResult(range, users);
        }

        private static IEnumerable<ItemCountResult> FormatResult(DateRangeViewModel range, List<ItemCountResult> items)
        {
            // Check if all days are included (even the days with 0 downloads)
            var totalDays = Math.Round((range.End - range.Start).TotalDays);
            if (items.Count < totalDays)
            {
                for (int i = 0; i <= totalDays; i++)
                {
                    if (items.FirstOrDefault(s => s.Date.Date.CompareTo(range.Start.AddDays(i).Date) == 0) == null)
                    {
                        items.Add(new ItemCountResult()
                        {
                            Date = range.Start.AddDays(i),
                            ItemCount = 0
                        });
                    }
                }

                items = items.OrderBy(s => s.Date).ToList();
            }
            return items;
        }

        private async Task<IEnumerable<FileTypeResult>> GetFileTypesCount(DateRangeViewModel range)
        {
            IEnumerable<FileTypesCountResult> queryResult = await _unitOfWork.Files.FileTypesByDateRange(range.Start, range.End);
            List<FileTypeResult> result = new List<FileTypeResult>();
            // Collect common file types
            foreach (var item in fileTypes)
            {
                result.Add(new FileTypeResult()
                {
                     FileType = item.Key,
                     ItemCount = queryResult.Where(s => fileTypes[item.Key].Contains(s.Extension)).Sum(s => s.ItemCount)
                });
            }
            // other file type
            result.Add(new FileTypeResult()
            {
                FileType = FileType.Others,
                ItemCount = queryResult.Where(s => !fileTypes[FileType.Archives].Contains(s.Extension))
                                       .Where(s => !fileTypes[FileType.Multimedia].Contains(s.Extension))
                                       .Where(s => !fileTypes[FileType.Documents].Contains(s.Extension))
                                       .Sum(s => s.ItemCount)
            });
            return result;
        }
    }
}
