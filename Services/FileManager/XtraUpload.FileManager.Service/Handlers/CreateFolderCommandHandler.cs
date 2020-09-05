using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using XtraUpload.Database.Data.Common;
using XtraUpload.Domain;
using XtraUpload.Domain.Infra;
using XtraUpload.FileManager.Service.Common;

namespace XtraUpload.FileManager.Service
{
    /// <summary>
    /// Create a new folder
    /// </summary>
    public class CreateFolderCommandHandler : IRequestHandler<CreateFolderCommand, CreateFolderResult>
    {
        #region Fields
        readonly IMediator _mediator;
        readonly IUnitOfWork _unitOfWork;
        readonly ClaimsPrincipal _caller;
        #endregion

        #region Constructor
        public CreateFolderCommandHandler(IUnitOfWork unitOfWork, IMediator mediator, IHttpContextAccessor httpContextAccessor)
        {
            _mediator = mediator;
            _unitOfWork = unitOfWork;
            _caller = httpContextAccessor.HttpContext.User;
        }
        #endregion

        #region Handler
        public async Task<CreateFolderResult> Handle(CreateFolderCommand request, CancellationToken cancellationToken)
        {
            string userId = _caller.GetUserId();

            FolderItem newFolder = new FolderItem()
            {
                Id = Helpers.GenerateUniqueId(),
                Name = request.FolderName,
                CreatedAt = DateTime.Now,
                IsAvailableOnline = true,
                LastModified = DateTime.Now,
                Parentid = request.ParentFolderId,
                UserId = userId
            };
            _unitOfWork.Folders.Add(newFolder);

            // Save to db
            CreateFolderResult Result = await _unitOfWork.CompleteAsync(new CreateFolderResult());
            if (Result.State == OperationState.Success)
            {
                Result.Folder = newFolder;
            }

            // Notify listners
            await _mediator.Publish(new FolderCreatedNotification(Result.Folder));

            return Result;
        }
        #endregion
    }
}
