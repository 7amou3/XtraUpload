using System.Threading.Tasks;
using XtraUpload.Domain;
using XtraUpload.ServerApp.Common;

namespace XtraUpload.Authentication.Service.Common
{
    public interface IAuthenticationService
    {
        Task<CreateUserAccountResult> CreateUserAccount(RegistrationViewModel model);
        Task<XuIdentityResult> StandardAuth(CredentialsViewModel credentials);
        Task<XuIdentityResult> SocialMediaAuth(SocialMediaLoginViewModel model);
        Task<OperationResult> LostPassword(string email, string clientIp);
        Task<OperationResult> CheckRecoveryInfo(string recoeryId);
        Task<OperationResult> RecoverPassword(RecoverPasswordViewModel model);
    }
}
