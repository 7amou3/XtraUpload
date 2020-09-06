using MediatR;
using XtraUpload.Domain;

namespace XtraUpload.Authentication.Service.Common
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
