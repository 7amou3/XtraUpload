using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using XtraUpload.Authentication.Service.Common;
using XtraUpload.Domain;
using XtraUpload.WebApp.Common;

namespace XtraUpload.Administration.Service.Common
{
    public interface IAdministrationService
    {
        /// <summary>
        /// Get a list of files 
        /// </summary>
        Task<PagingResult<FileItemExtended>> GetFiles(PageSearchModel model);

        /// <summary>
        /// Get a list of file extensions
        /// </summary>
        Task<FileExtensionsResult> GetFileExtensions();

        /// <summary>
        /// Search for users by name
        /// </summary>
        Task<SearchUserResult> SearchUsers(string name);

        /// <summary>
        /// Add an extension
        /// </summary>
        Task<FileExtensionResult> AddExtension(string name);


        /// <summary>
        /// Delete file(s) by id
        /// </summary>
        Task<DeleteFilesResult> DeleteFiles(IEnumerable<string> ids);

        /// <summary>
        /// Get the users role
        /// </summary>
        Task<RolesResult> GetUsersRole();


        /// <summary>
        /// Delete a role claim
        /// </summary>
        Task<OperationResult> DeleteRoleClaims(string roleId);

        /// <summary>
        /// Get all pages
        /// </summary>
        Task<PagesResult> GetPages();

        /// <summary>
        /// Add a new page
        /// </summary>
        Task<PageResult> AddPage(Page page);

        /// <summary>
        /// Update a page
        /// </summary>
        Task<PageResult> UpdatePage(Page page);

        /// <summary>
        /// Delete a Page
        /// </summary>
        Task<OperationResult> DeletePage(string Id);
    }
}
