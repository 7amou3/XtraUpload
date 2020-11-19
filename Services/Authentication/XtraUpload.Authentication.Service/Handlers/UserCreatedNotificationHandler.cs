using MediatR;
using System.Threading;
using System.Threading.Tasks;
using XtraUpload.Authentication.Service.Common;
using XtraUpload.Database.Data.Common;
using XtraUpload.Domain;

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
            await _unitOfWork.Folders.AddRangeAsync(Helpers.GenerateDefaultFolders(notification.User.Id));

            // Save to db
            await _unitOfWork.CompleteAsync();
        }
    }
}
