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

        public AdministrationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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
