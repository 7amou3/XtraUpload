using MediatR;
using XtraUpload.Domain;

namespace XtraUpload.Authentication.Service.Common
{
    public class ResetPasswordCommand: IRequest<OperationResult>
    {
        public ResetPasswordCommand(string email, string clientIp)
        {
            Email = email;
            ClientIp = clientIp;
        }
        public string Email { get; }
        public string ClientIp { get; }
    }
}
