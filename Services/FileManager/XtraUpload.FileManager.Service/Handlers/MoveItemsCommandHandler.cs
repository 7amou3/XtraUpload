using Askmethat.Aspnet.JsonLocalizer.Localizer;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using XtraUpload.Database.Data.Common;
using XtraUpload.Domain;
using XtraUpload.FileManager.Service.Common;

namespace XtraUpload.FileManager.Service
{
    /// <summary>
    /// Move the selected items to the specified folder
    /// </summary>
    public class MoveItemsCommandHandler : IRequestHandler<MoveItemsCommand, MoveItemsResult>
    {
        #region Fields
        readonly IMediator _mediatr;
        readonly IUnitOfWork _unitOfWork;
        readonly ClaimsPrincipal _caller;
        readonly IJsonStringLocalizer _localizer;
        #endregion

        #region Constructor
        public MoveItemsCommandHandler(IUnitOfWork unitOfWork, IMediator mediatr,
            IJsonStringLocalizer localizer, IHttpContextAccessor httpContextAccessor)
        {
            _mediatr = mediatr;
            _localizer = localizer;
            _unitOfWork = unitOfWork;
            _caller = httpContextAccessor.HttpContext.User;
        }
        #endregion

        #region Handler
        public async Task<MoveItemsResult> Handle(MoveItemsCommand request, CancellationToken cancellationToken)
        {
            MoveItemsResult Result = new MoveItemsResult();

            List<string> movedIds = new List<string>();
            string userId = _caller.GetUserId();

            FolderItem destFolder = await _unitOfWork.Folders.FirstOrDefaultAsync(s => s.Id == request.DestFolderId);
            // Check destination folder exists
            if (destFolder == null && request.DestFolderId != "root")
            {
                Result.ErrorContent = new ErrorContent(_localizer["No folder with the provided id was found"], ErrorOrigin.Client);
                return Result;
            }

            if (request.SelectedFolders.Any())
            {
                // Check if user not trying to move folders to same location
                if (request.SelectedFolders.Any(s => s.Parentid == request.DestFolderId || s.Id == request.DestFolderId))
                {
                    Result.ErrorContent = new ErrorContent(_localizer["Moving folders to the same location is not possible!"], ErrorOrigin.Client);
                    return Result;
                }
                // Get the child folders of the currently moved folder(s)
                List<FolderItem> childFolders = new List<FolderItem>();
                foreach (FolderItem f in request.SelectedFolders)
                {
                    childFolders.AddRange(await _mediatr.Send(new GetFoldersRecursivelyQuery(f)));
                }
                childFolders.RemoveAt(childFolders.Count - 1);
                if (childFolders.Any(s => s.Id == request.DestFolderId))
                {
                    Result.ErrorContent = new ErrorContent(_localizer["Moving a parent folder to one of its child folders is not possible"], ErrorOrigin.Client);
                    return Result;
                }

                var foldersId = request.SelectedFolders.Select(s => s.Id);
                var folders = await _unitOfWork.Folders.FindAsync(s => foldersId.Contains(s.Id));
                foreach (FolderItem folder in folders)
                {
                    folder.Parentid = request.DestFolderId;
                    movedIds.Add(folder.Id);
                }
            }
            if (request.SelectedFiles.Any())
            {
                // Check if user not trying to move files to same location
                if (request.SelectedFiles.Any(s => s.FolderId == request.DestFolderId || (s.FolderId == null && request.DestFolderId == "root")))
                {
                    Result.ErrorContent = new ErrorContent(_localizer["Moving files to the same location is not possible!"], ErrorOrigin.Client);
                    return Result;
                }
                IEnumerable<string> filesId = request.SelectedFiles.Select(s => s.Id);
                IEnumerable<FileItem> files = await _unitOfWork.Files.FindAsync(s => filesId.Contains(s.Id));

                // Update file's folder id
                foreach (FileItem file in files)
                {
                    file.FolderId = request.DestFolderId == "root" ? null : request.DestFolderId;
                    movedIds.Add(file.Id);
                }
            }

            // Try to save to db
            Result = await _unitOfWork.CompleteAsync(Result);
            if (Result.State == OperationState.Success)
            {
                Result.MovedItemsIds = movedIds;
                // Get the new folder structure
                Result.Folders = await _unitOfWork.Folders.FindAsync(f => f.UserId == userId);
            }

            return Result;
        }
        #endregion
    }
}
