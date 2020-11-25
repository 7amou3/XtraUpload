using MediatR;
using XtraUpload.Domain;

namespace XtraUpload.Authentication.Service.Common
{
    public class CreateUserCommand: IRequest<CreateUserResult>
    {
        public CreateUserCommand(User user, Language language)
        {
            User = user;
            Language = language;
        }
        public User User { get; }
        public Language Language { get; }
    }
}
