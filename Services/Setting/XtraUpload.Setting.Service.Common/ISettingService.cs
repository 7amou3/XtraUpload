using System;
using System.Threading.Tasks;
using XtraUpload.Administration.Service.Common;
using XtraUpload.Domain;
using XtraUpload.WebApp.Common;

namespace XtraUpload.Setting.Service.Common
{
    public interface ISettingService
    {
        /// <summary>
        /// Updates a user password
        /// </summary>
        Task<UpdatePasswordResult> UpdatePassword(UpdatePassword model);

        /// <summary>
        /// Get a page by name
        /// </summary>
        Task<PageResult> GetPage(string pageName);
    }
}
