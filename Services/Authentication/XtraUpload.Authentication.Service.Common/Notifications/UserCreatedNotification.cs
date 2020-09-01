using MediatR;
using XtraUpload.Domain;

namespace XtraUpload.Authentication.Service.Common
{
    /// <summary>
    /// Notifcation raised when a user created
    /// </summary>
    public class UserCreatedNotification: INotification
    {
        public User User { get; }

        public UserCreatedNotification(User user)
        {
            User = user;
        }
    }
}
