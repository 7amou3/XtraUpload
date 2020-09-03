using MediatR;
using XtraUpload.Domain;

namespace XtraUpload.Authentication.Service.Common
{
    public class ValidatePwdTokenCommand : IRequest<OperationResult>
    {
        public ValidatePwdTokenCommand(string newPassword, string recoveryKey)
        {
            NewPassword = newPassword;
            RecoveryKey = recoveryKey;
        }
        public string NewPassword { get; }

        public string RecoveryKey { get; }
    }
}
