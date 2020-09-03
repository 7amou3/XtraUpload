using System;
using XtraUpload.Email.Service;
using XtraUpload.Email.Service.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MediatR;
using XtraUpload.Domain.Infra;

namespace XtraUpload.Email.Host
{
    public static class Startup
    {
        public static void AddEmail(this IServiceCollection services, IConfiguration config)
        {
            // Health check
            services.AddHealthChecks().AddCheck<EmailHealthCheckHandler>("Email Service");

            // Config Options
            IConfigurationSection emailSection = config.GetSection(nameof(EmailSettings));
            services.Configure<EmailSettings>(emailSection);
            services.ConfigureWritable<EmailSettings>(config.GetSection(nameof(EmailSettings)));

            // Add mediatr (no need to register all handlers, mediatr will scan the assembly and register them automatically)
            services.AddMediatR(typeof(UserCreatedNotificationHandler));
        }
    }
}
