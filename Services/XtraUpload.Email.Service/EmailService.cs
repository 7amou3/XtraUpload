using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using XtraUpload.Domain;
using XtraUpload.Email.Service.Common;
using XtraUpload.ServerApp.Common;

namespace XtraUpload.Email.Service
{
    public class EmailService : IEmailService, IHealthCheck
    {
        #region Fields
        readonly HttpRequest _request;
        readonly EmailSettings _emailSetting;
        readonly ILogger<EmailService> _logger;
        readonly IBackgroundTaskQueue _backgroundTaskQueue;
        #endregion

        #region Constructor
        public EmailService(IBackgroundTaskQueue backgroundTaskQueue, IOptionsMonitor<EmailSettings> emailSetting,
            IHttpContextAccessor context, ILogger<EmailService> logger)
        {
            _logger = logger;
            _request = context.HttpContext.Request;
            _emailSetting = emailSetting.CurrentValue;
            _backgroundTaskQueue = backgroundTaskQueue;
        }
        #endregion

        /// <summary>
        /// Application base url
        /// </summary>
        string BaseUrl
        {
            get
            {
                return _request.Scheme + "://" + _request.Host + _request.PathBase;
            }
        }

        /// <summary>
        /// Send a mail to confirm a user's email address
        /// </summary>
        public void SendConfirmEmail(ConfirmationKey emailKey, User to)
        {
            EmailMessage template = GetConfirmEmailTpl(emailKey, to);
           
            // Enqueue to background service
            _backgroundTaskQueue.QueueBackgroundWorkItem((token) => Task.Run(() => Send(to, template, token)));
        }

        /// <summary>
        /// Send a recovery email password to the requested user
        /// </summary>
        public void SendPassRecovery(ConfirmationKey pwdReset, User to)
        {
            EmailMessage template = GetResetPasswordTpl(pwdReset, to);
            
            // Enqueue to background service
            _backgroundTaskQueue.QueueBackgroundWorkItem((token) => Task.Run(() => Send(to, template, token)));
        }

        private void Send(User to, EmailMessage template, CancellationToken token)
        {
            try
            {
                MimeMessage message = new MimeMessage()
                {
                    Subject = template.Subject,
                    Body = new TextPart(TextFormat.Html) { Text = template.HtmlContent }
                };
                message.To.Add(new MailboxAddress(to.UserName, to.Email));
                message.From.Add(new MailboxAddress(_emailSetting.Sender.Name, _emailSetting.Sender.Admin));

                #region Trace
                _logger.LogInformation("Sending email from background service is starting.");
                #endregion
                using var client = new SmtpClient();
                client.Connect(_emailSetting.Smtp.Server, _emailSetting.Smtp.Port, true, token);
                //Remove any OAuth functionality as we won't be using it. 
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                client.Authenticate(_emailSetting.Smtp.Username, _emailSetting.Smtp.Password, token);
                client.Send(message, token);
                client.Disconnect(true, token);
                #region Trace
                _logger.LogInformation("Sending email from background service is complete.");
                #endregion
            }
            catch (Exception ex)
            {
                #region Trace
                _logger.LogError(ex.Message);
                #endregion
            }
        }

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

        private EmailMessage GetResetPasswordTpl(ConfirmationKey pwdReset, User user)
        {
            string templateName = "ResetPassword.html";
            string templatePath = GetTemplatePath(templateName);
            string recoveryLink = BaseUrl + "/recoverpassword/" + pwdReset.Id;

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

        private string GetTemplatePath(string templateName)
        {
            string assemblyName = typeof(EmailService).Assembly.GetName().Name;
            string baseDirectory = Path.GetDirectoryName(Environment.CurrentDirectory);
            string templatePath = Path.Combine(baseDirectory, "Services", assemblyName, "Templates", templateName);

            if (!File.Exists(templatePath))
            {
                _logger.LogError("No email template found at: " + templatePath);
            }
            return templatePath;
        }

        #region IHealthCheck members

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                using var client = new SmtpClient();
                client.Connect(_emailSetting.Smtp.Server, _emailSetting.Smtp.Port, true, cancellationToken);
                client.Authenticate(_emailSetting.Smtp.Username, _emailSetting.Smtp.Password, cancellationToken);

                if (!client.IsConnected)
                {
                    return Task.FromResult(HealthCheckResult.Degraded("Failed to connect to the mail server."));
                }
                if (!client.IsAuthenticated)
                {
                    return Task.FromResult(HealthCheckResult.Degraded("Authentication failed."));
                }
                
                return Task.FromResult(HealthCheckResult.Healthy());
                
            }
            catch (Exception)
            {
                // Do not return the exception message, it may carry account sensitive data..
                return Task.FromResult(HealthCheckResult.Degraded("Cannot connect to the mail server, please check your mail configuratoin."));
            }
        }

        #endregion
    }
}
