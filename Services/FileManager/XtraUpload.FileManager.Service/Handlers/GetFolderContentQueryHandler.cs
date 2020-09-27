using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using XtraUpload.Database.Data.Common;
using XtraUpload.Domain;
using XtraUpload.Domain.Infra;
using XtraUpload.FileManager.Service.Common;

namespace XtraUpload.FileManager.Service
{
    public class GetFolderContentQueryHandler : IRequestHandler<GetFolderContentQuery, GetFolderContentResult>
    {
        #region Fields
        readonly IUnitOfWork _unitOfWork;
        readonly ClaimsPrincipal _caller;
        #endregion

        #region Constructor
        public GetFolderContentQueryHandler(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _caller = httpContextAccessor.HttpContext.User;
        }
        #endregion

        #region Handler
        public async Task<GetFolderContentResult> Handle(GetFolderContentQuery request, CancellationToken cancellationToken)
        {
            string userId = _caller.GetUserId();
            // user can pass null as root folder
            string parentid = request.FolderId ?? "root";

            Expression<Func<FileItem, bool>> criteria = s => s.FolderId == request.FolderId && s.UserId == userId;

            GetFolderContentResult Result = new GetFolderContentResult()
            {
                // Get folders
                Folders = await _unitOfWork.Folders.FindAsync(s => s.Parentid == parentid && s.UserId == userId),
                // get Files, the root folder is represented by a null value in TFile table
                Files = await _unitOfWork.Files.GetFilesServerInfo(criteria)
            };

            return Result;
        }
        #endregion
    }
}
