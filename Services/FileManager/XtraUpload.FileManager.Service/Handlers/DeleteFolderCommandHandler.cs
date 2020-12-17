using Askmethat.Aspnet.JsonLocalizer.Localizer;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XtraUpload.Database.Data.Common;
using XtraUpload.Domain;
using XtraUpload.FileManager.Service.Common;

namespace XtraUpload.FileManager.Service.Handlers
{
    /// <summary>
    /// Delete a folder and all it's content
    /// </summary>
    public class DeleteFolderCommandHandler : IRequestHandler<DeleteFolderCommand, DeleteFolderResult>
    {
        readonly IMediator _mediatr;
        readonly IUnitOfWork _unitOfWork;
        readonly ClaimsPrincipal _caller;
        readonly IJsonStringLocalizer _localizer;

        public DeleteFolderCommandHandler(IUnitOfWork unitOfWork, IMediator mediatr, 
            IHttpContextAccessor httpContextAccessor, IJsonStringLocalizer localizer)
        {
            _mediatr = mediatr;
            _unitOfWork = unitOfWork;
            _localizer = localizer;
            _caller = httpContextAccessor.HttpContext.User;
        }
        
        public async Task<DeleteFolderResult> Handle(DeleteFolderCommand request, CancellationToken cancellationToken)
        {
            DeleteFolderResult Result = new DeleteFolderResult();

            string userId = _caller.GetUserId();
            FolderItem folder = await _unitOfWork.Folders.FirstOrDefaultAsync(s => s.Id == request.FolderId);

            // Check if file exist
            if (folder == null)
            {
                Result.ErrorContent = new ErrorContent(_localizer["No folder with the provided id was found"], ErrorOrigin.Client);
                return Result;
            }

            IEnumerable<FolderItem> folders = await _mediatr.Send(new GetFoldersRecursivelyQuery(folder));

            // Extract folders id 
            List<string> foldersId = new List<string>() { folder.Id };
            foldersId.AddRange(folders.Select(s => s.Id).ToList());

            // Get all files in those folders
            var files = await _unitOfWork.Files.FindAsync(s => s.UserId == userId && foldersId.Contains(s.FolderId));
            // Mark files to be deleted
            foreach (var file in files)
            {
                file.Status = ItemStatus.To_Be_Deleted;
            }
            // Delete folders from db
            _unitOfWork.Folders.Remove(folder);
            _unitOfWork.Folders.RemoveRange(folders);
            
            // Persist changes
            Result = await _unitOfWork.CompleteAsync(Result);
            if (Result.State == OperationState.Success)
            {
                Result.Folders = folders;
                Result.Files = files;
            }

            return Result;
        }
    }
}
