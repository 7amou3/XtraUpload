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
