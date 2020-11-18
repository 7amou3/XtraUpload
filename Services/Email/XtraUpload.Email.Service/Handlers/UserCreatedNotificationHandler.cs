using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using XtraUpload.Authentication.Service.Common;
using XtraUpload.Database.Data.Common;
using XtraUpload.Domain;
using XtraUpload.Domain.Infra;

namespace XtraUpload.Email.Service
{
    /// <summary>
    /// Raised when a user gets created
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
            // Store a confirmation email token
            await _unitOfWork.ConfirmationKeys.AddAsync(new ConfirmationKey()
            {
                GenerateAt = DateTime.Now,
                Id = Helpers.GenerateUniqueId(),
                Status = RequestStatus.InProgress,
                UserId = notification.User.Id
            });

            // Save to db
            await _unitOfWork.CompleteAsync();

            // Todo: send welcome email with confirmation code
        }
    }
}
