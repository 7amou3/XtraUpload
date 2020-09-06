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
        /// Search for users by name
        /// </summary>
        Task<SearchUserResult> SearchUsers(string name);


        /// <summary>
        /// Get the users role
        /// </summary>
        Task<RolesResult> GetUsersRole();


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
