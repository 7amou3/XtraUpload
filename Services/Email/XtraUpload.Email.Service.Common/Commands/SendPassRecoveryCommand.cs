using MediatR;
using XtraUpload.Domain;

namespace XtraUpload.Email.Service.Common
{
    public class SendPassRecoveryCommand: IRequest
    {
        public SendPassRecoveryCommand(User to, ConfirmationKey tokenKey)
        {
            To = to;
            TokenKey = tokenKey;
        }
        public User To { get; }
        public ConfirmationKey TokenKey { get; }
    }
}
