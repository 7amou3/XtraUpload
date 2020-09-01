using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using XtraUpload.Authentication.Service.Common;
using XtraUpload.Database.Data.Common;
using XtraUpload.Domain;
using XtraUpload.Domain.Infra;

namespace XtraUpload.Authentication.Service
{
    /// <summary>
    /// Raised when a user gets created
    /// todo: move to filemanager service
    /// </summary>
    public class UserCreatedNotificationHandler : INotificationHandler<UserCreatedNotification>
    {
        readonly IUnitOfWork _unitOfWork;

        public UserCreatedNotificationHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(UserCreatedNotification notification, CancellationToken cancellationToken)
        {
            // Create default folders tree for this user
            _unitOfWork.Folders.AddRange(Helpers.GenerateDefaultFolders(notification.User.Id));

            // Save to db
            await _unitOfWork.CompleteAsync();
        }
    }
}
