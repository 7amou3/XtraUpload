using System;
using XtraUpload.Email.Service;
using XtraUpload.Email.Service.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace XtraUpload.Email.Host
{
    public static class Startup
    {
        public static void AddEmail(this IServiceCollection services, IConfiguration config)
        {
            // Service
            services.AddScoped<IEmailService, EmailService>();

            // Health check
            services.AddHealthChecks().AddCheck<EmailService>("Email Service");

            // Config Options
            IConfigurationSection emailSection = config.GetSection(nameof(EmailSettings));
            services.Configure<EmailSettings>(emailSection);
        }
    }
}
