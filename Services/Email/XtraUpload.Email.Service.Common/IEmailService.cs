using System;
using System.Threading.Tasks;
using XtraUpload.Domain;

namespace XtraUpload.Email.Service.Common
{
    public interface IEmailService
    {
        /// <summary>
        /// Send a mail to confirm a user's email address
        /// </summary>
        void SendConfirmEmail(ConfirmationKey emailKey, User to);

        /// <summary>
        /// Send a recovery email password to the requested user
        /// </summary>
        void SendPassRecovery(ConfirmationKey pwdReset, User to);
    }
}
