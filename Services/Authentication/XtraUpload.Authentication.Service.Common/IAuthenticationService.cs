using System.Threading.Tasks;
using XtraUpload.Domain;
using XtraUpload.WebApp.Common;

namespace XtraUpload.Authentication.Service.Common
{
    public interface IAuthenticationService
    {
        Task<OperationResult> LostPassword(string email, string clientIp);
        Task<OperationResult> CheckRecoveryInfo(string recoeryId);
        Task<OperationResult> RecoverPassword(RecoverPasswordViewModel model);

    }
}
