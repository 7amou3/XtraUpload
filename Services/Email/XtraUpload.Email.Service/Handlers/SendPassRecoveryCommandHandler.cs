using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using XtraUpload.Domain;
using XtraUpload.Email.Service.Common;

namespace XtraUpload.Email.Service.Handlers
{
    /// <summary>
    /// Sends a recovery email password to the requested user
    /// </summary>
    public class SendPassRecoveryCommandHandler : EmailSenderBase, IRequestHandler<SendPassRecoveryCommand>
    {
        #region Fields
        readonly EmailSettings _emailSetting;
        readonly IBackgroundTaskQueue _backgroundTaskQueue;
        #endregion

        #region Constructor
        public SendPassRecoveryCommandHandler(IBackgroundTaskQueue backgroundTaskQueue, IHttpContextAccessor context, IOptionsMonitor<EmailSettings> emailSetting, ILogger<SendPassRecoveryCommandHandler> logger)
            : base (context, emailSetting.CurrentValue, logger)
        {
            _emailSetting = emailSetting.CurrentValue;
            _backgroundTaskQueue = backgroundTaskQueue;
        }
        #endregion

        #region Handler
        public Task<Unit> Handle(SendPassRecoveryCommand request, CancellationToken cancellationToken)
        {
            EmailMessage template = GetResetPasswordTpl(request.TokenKey, request.To);

            // Enqueue to background service to free the caller
            _backgroundTaskQueue.QueueBackgroundWorkItem((token) => Task.Run(() => Send(request.To, template, token)));

            return Task.FromResult(Unit.Value);
        }
        #endregion

        #region Helpers
        private EmailMessage GetResetPasswordTpl(ConfirmationKey token, User user)
        {
            string templateName = "ResetPassword.html";
            string templatePath = GetTemplatePath(templateName);
            string recoveryLink = BaseUrl + "/recoverpassword/" + token.Id;

            using StreamReader SourceReader = File.OpenText(templatePath);
            BodyBuilder builder = new BodyBuilder()
            {
                HtmlBody = SourceReader.ReadToEnd()
            };
            EmailMessage message = new EmailMessage
            {
                Subject = "Reset Your Email",
                HtmlContent = string.Format(builder.HtmlBody, user.UserName, _emailSetting.Sender.Name, _emailSetting.Sender.Support, recoveryLink)
            };

            return message;
        }
        #endregion
    }
}
