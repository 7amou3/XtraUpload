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
using XtraUpload.Domain.Infra;
using XtraUpload.FileManager.Service.Common;

namespace XtraUpload.FileManager.Service
{
    /// <summary>
    /// Delete folders and files
    /// </summary>
    public class DeleteItemsCommandHandler : IRequestHandler<DeleteItemsCommand, DeleteItemsResult>
    {
        #region Fields
        readonly IMediator _mediator;
        readonly IUnitOfWork _unitOfWork;
        readonly ClaimsPrincipal _caller;
        readonly UploadOptions _uploadOpt;
        #endregion

        #region Constructor
        public DeleteItemsCommandHandler(
            IUnitOfWork unitOfWork, 
            IMediator mediator,
            IOptionsMonitor<UploadOptions> uploadOpt,
            IHttpContextAccessor httpContextAccessor)
        {
            _mediator = mediator;
            _unitOfWork = unitOfWork;
            _uploadOpt = uploadOpt.CurrentValue;
            _caller = httpContextAccessor.HttpContext.User;
        }
        #endregion

        #region Handler


        public async Task<DeleteItemsResult> Handle(DeleteItemsCommand request, CancellationToken cancellationToken)
        {
            DeleteItemsResult result = new DeleteItemsResult();
            IEnumerable<FileItem> files = new List<FileItem>();
            List<DeleteFolderResult> folders = new List<DeleteFolderResult>();

            string userId = _caller.GetUserId();
            // Delete files
            if (request.SelectedFiles.Any())
            {
                List<string> filesIds = request.SelectedFiles.Select(s => s.Id).ToList();
                files = await _unitOfWork.Files.FindAsync(s => filesIds.Contains(s.Id) && s.UserId == userId);
                _unitOfWork.Files.RemoveRange(files);
            }
            // Delete folders
            if (request.SelectedFolders.Any())
            {
                foreach (var folder in request.SelectedFolders)
                {
                    folders.Add(await _mediator.Send(new DeleteFolderCommand(folder.Id)));
                }
            }

            // Persist changes
            result = await _unitOfWork.CompleteAsync(result);
            if (result.State == OperationState.Success)
            {
                // Remove the files from the drive (no need to queue to background thread, because Directory.Delete does not block)
                foreach (var file in files)
                {
                    string folderPath = Path.Combine(_uploadOpt.UploadPath, file.UserId, file.Id);

                    if (Directory.Exists(folderPath))
                    {
                        Directory.Delete(folderPath, true);
                    }
                }
                // Append new data
                result.Folders = folders;
                result.Files = files;
            }

            return result;
        }
        #endregion
    }
}
