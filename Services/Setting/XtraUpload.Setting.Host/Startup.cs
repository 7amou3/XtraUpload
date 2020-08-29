using System;
using XtraUpload.Setting.Service;
using XtraUpload.Setting.Service.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace XtraUpload.Setting.Host
{
    public static class Startup
    {
        public static void AddXtraUploadSetting(this IServiceCollection services, IConfiguration config)
        {
            // Register services
            services.AddScoped<ISettingService, SettingService>();
            services.AddScoped<IAppSettingsService, AppSettingsService>();

            // Health check
            services.AddHealthChecks()
                .AddCheck<MemoryHealthCheck>("Memory")
                .AddCheck<StorageHealthCheck>("Storage Space");

            // config options
            RegisterOptions(services, config);
        }

        /// <summary>
        /// Deserialize configs from appsettings and store them in DI
        /// </summary>
        private static void RegisterOptions(IServiceCollection services, IConfiguration config)
        {
            // WebApp settings
            IConfigurationSection WebAppSection = config.GetSection(nameof(WebAppSettings));
            services.Configure<WebAppSettings>(WebAppSection);

            // Hardware Options
            IConfigurationSection hardwareSection = config.GetSection(nameof(HardwareCheckOptions));
            services.Configure<HardwareCheckOptions>(hardwareSection);
        }

    }
}
