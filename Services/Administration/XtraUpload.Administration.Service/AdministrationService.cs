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
using XtraUpload.FileManager.Service.Common;
using XtraUpload.WebApp.Common;

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

        
        /// <summary>
        /// Edit a user
        /// </summary>
        public async Task<EditUserResult> EditUser(EditUserCommand model)
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
            return await _unitOfWork.CompleteAsync(result);
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
            result = await _unitOfWork.CompleteAsync(result);

            if (result.State == OperationState.Success)
            {
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

            return result;
        }

        /// <summary>
        /// Get a list of files 
        /// </summary>
        public async Task<PagingResult<FileItemExtended>> GetFiles(PageSearchModel model)
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
            result = await _unitOfWork.CompleteAsync(result);
            if (result.State == OperationState.Success)
            {
                result.FileExtension = newFileType;
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
            return await _unitOfWork.CompleteAsync(result);
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
                // Remove from collection
                _unitOfWork.Files.RemoveRange(files);

                // Save to db
                result = await _unitOfWork.CompleteAsync(result);
                if (result.State == OperationState.Success)
                {
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
            return await _unitOfWork.CompleteAsync(result);
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
            result = await _unitOfWork.CompleteAsync(result);
            if (result.State == OperationState.Success)
            {
                result.Page = p;
            }

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
            result = await _unitOfWork.CompleteAsync(result);
            if (result.State == OperationState.Success)
            {
                result.Page = page;
            }
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
            return await _unitOfWork.CompleteAsync(result);
        }

        
    }
}
