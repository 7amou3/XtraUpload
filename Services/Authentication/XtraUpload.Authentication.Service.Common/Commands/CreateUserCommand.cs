using MediatR;
using XtraUpload.Authentication.Service.Common;
using XtraUpload.Domain;

namespace XtraUpload.WebApp.Common
{
    public class CreateUserCommand: IRequest<CreateUserResult>
    {
        public CreateUserCommand(User user)
        {
            User = user;
        }
        public User User { get; }
    }
}
