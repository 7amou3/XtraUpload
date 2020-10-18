using System;
using XtraUpload.Setting.Service;
using XtraUpload.Setting.Service.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MediatR;
using XtraUpload.Domain.Infra;
using XtraUpload.Domain;

namespace XtraUpload.Setting.Host
{
    public static class Startup
    {
        public static void AddXtraUploadSetting(this IServiceCollection services, IConfiguration config)
        {
            // Health check
            services.AddHealthChecks().AddCheck<MemoryHealthCheck>("Memory");

            // Add mediatr (no need to register all handlers, mediatr will scan the assembly and register them automatically)
            services.AddMediatR(typeof(RequestConfirmationEmailCommandHandler));

            // config options
            RegisterOptions(services, config);
        }

        /// <summary>
        /// Deserialize configs from appsettings and store them in DI
        /// </summary>
        private static void RegisterOptions(IServiceCollection services, IConfiguration config)
        {
            // WebApp settings
            IConfigurationSection webAppSection = config.GetSection(nameof(WebAppSettings));
            services.Configure<WebAppSettings>(webAppSection);
            services.ConfigureWritable<WebAppSettings>(webAppSection);

            // Hardware Options
            IConfigurationSection hardwareSection = config.GetSection(nameof(HardwareCheckOptions));
            services.Configure<HardwareCheckOptions>(hardwareSection);
            services.ConfigureWritable<HardwareCheckOptions>(hardwareSection);
        }

    }
}
