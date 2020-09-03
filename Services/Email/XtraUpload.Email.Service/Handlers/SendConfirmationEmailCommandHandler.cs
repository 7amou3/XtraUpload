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

namespace XtraUpload.Email.Service
{
    /// <summary>
    /// Sends an email to confirm a user's email address
    /// </summary>
    public class SendConfirmationEmailCommandHandler : EmailSenderBase, IRequestHandler<SendConfirmationEmailCommand>
    {
        #region Fields
        readonly EmailSettings _emailSetting;
        readonly IBackgroundTaskQueue _backgroundTaskQueue;
        #endregion

        #region Constructor
        public SendConfirmationEmailCommandHandler(IBackgroundTaskQueue backgroundTaskQueue, IHttpContextAccessor context, IOptionsMonitor<EmailSettings> emailSetting, ILogger<SendConfirmationEmailCommandHandler> logger)
            : base(context, emailSetting.CurrentValue, logger)
        {
            _emailSetting = emailSetting.CurrentValue;
            _backgroundTaskQueue = backgroundTaskQueue;
        }
        #endregion

        #region Handler
        public Task<Unit> Handle(SendConfirmationEmailCommand request, CancellationToken cancellationToken)
        {
            EmailMessage template = GetConfirmEmailTpl(request.TokenKey, request.To);

            // Enqueue to background service
            _backgroundTaskQueue.QueueBackgroundWorkItem((token) => Task.Run(() => Send(request.To, template, token)));

            return Task.FromResult(Unit.Value);
        }
        #endregion

        #region Helpers
        private EmailMessage GetConfirmEmailTpl(ConfirmationKey emailKey, User user)
        {
            string templateName = "ConfirmEmail.html";
            string templatePath = GetTemplatePath(templateName);
            string emailConfirmationLink = BaseUrl + "/confirmemail/" + emailKey.Id;

            using StreamReader SourceReader = File.OpenText(templatePath);
            BodyBuilder builder = new BodyBuilder()
            {
                HtmlBody = SourceReader.ReadToEnd()
            };
            EmailMessage message = new EmailMessage
            {
                Subject = "Confirm Your Email",
                HtmlContent = string.Format(builder.HtmlBody, user.UserName, _emailSetting.Sender.Name, _emailSetting.Sender.Support, emailConfirmationLink)
            };

            return message;
        }
        #endregion
    }
}
