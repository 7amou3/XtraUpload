using MediatR;
using System;
using System.Threading.Tasks;
using System.Threading;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using XtraUpload.Email.Service.Common;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace XtraUpload.Email.Service
{
    public class EmailHealthCheckHandler : IHealthCheck, IRequestHandler<EmailHealthCheckQuery, HealthCheckResult>
    {
        #region Fields
        readonly EmailSettings _emailSetting;
        readonly ILogger<EmailHealthCheckHandler> _logger;
        #endregion

        #region Constructor
        public EmailHealthCheckHandler(IOptionsMonitor<EmailSettings> emailSetting, ILogger<EmailHealthCheckHandler> logger)
        {
            _logger = logger;
            _emailSetting = emailSetting.CurrentValue;
        }
        #endregion

        #region Handler
        public Task<HealthCheckResult> Handle(EmailHealthCheckQuery request, CancellationToken cancellationToken)
        {
            return CheckHealthAsync(null, cancellationToken);
        }
        #endregion

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
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                // Do not return the exception message, it may carry account sensitive data..
                return Task.FromResult(HealthCheckResult.Degraded("Cannot connect to the mail server, please check your mail configuratoin."));
            }
        }
        #endregion
    }
}
