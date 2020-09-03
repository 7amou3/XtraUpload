using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MimeKit;
using MimeKit.Text;
using System;
using System.IO;
using System.Reflection;
using System.Threading;
using XtraUpload.Domain;
using XtraUpload.Email.Service.Common;

namespace XtraUpload.Email.Service
{
    public abstract class EmailSenderBase
    {
        #region Fields
        readonly HttpRequest _request;
        readonly EmailSettings _emailSetting;
        readonly ILogger<EmailSenderBase> _logger;
        #endregion

        #region Constructor
        protected EmailSenderBase(IHttpContextAccessor context, EmailSettings emailSetting, ILogger<EmailSenderBase> logger)
        {
            _logger = logger;
            _emailSetting = emailSetting;
            _request = context.HttpContext.Request;
        }
        #endregion


        /// <summary>
        /// Application base url
        /// </summary>
        protected string BaseUrl
        {
            get
            {
                return _request.Scheme + "://" + _request.Host + _request.PathBase;
            }
        }

        /// <summary>
        /// Send an email to a user
        /// </summary>
        protected void Send(User to, EmailMessage template, CancellationToken token)
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

        protected string GetTemplatePath(string templateName)
        {
            var dirPath = Assembly.GetExecutingAssembly().Location;
            dirPath = Path.GetDirectoryName(dirPath);
            string templatePath = Path.GetFullPath(Path.Combine(dirPath, "EmailTemplates", templateName));

            if (!File.Exists(templatePath))
            {
                _logger.LogError("No email template found at: " + templatePath);
            }
            return templatePath;
        }
    }
}
