using MediatR;
using Microsoft.AspNetCore.Http;
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

namespace XtraUpload.FileManager.Service
{
    /// <summary>
    /// Delete folders and mark files for deletion
    /// </summary>
    public class DeleteItemsCommandHandler : IRequestHandler<DeleteItemsCommand, DeleteItemsResult>
    {
        readonly IMediator _mediator;
        readonly IUnitOfWork _unitOfWork;
        readonly ClaimsPrincipal _caller;
        
        public DeleteItemsCommandHandler(
            IUnitOfWork unitOfWork, 
            IMediator mediator,
            IHttpContextAccessor httpContextAccessor)
        {
            _mediator = mediator;
            _unitOfWork = unitOfWork;
            _caller = httpContextAccessor.HttpContext.User;
        }
        
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
                // mark files to be deleted
                foreach (FileItem file in files)
                {
                    file.Status = ItemStatus.To_Be_Deleted;
                }
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
                result.Folders = folders;
                result.Files = files;
            }

            return result;
        }
    }
}
