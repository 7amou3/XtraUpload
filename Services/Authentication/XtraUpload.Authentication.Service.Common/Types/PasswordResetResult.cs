using XtraUpload.Domain;

namespace XtraUpload.Authentication.Service.Common
{
    public class PasswordResetResult: OperationResult
    {
        public ConfirmationKey PwdReset { get; set; }
    }
}
