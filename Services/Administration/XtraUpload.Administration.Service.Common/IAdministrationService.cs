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
        /// Delete a Page
        /// </summary>
        Task<OperationResult> DeletePage(string Id);
    }
}
