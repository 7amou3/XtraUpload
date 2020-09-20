using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SixLabors.ImageSharp.Web.DependencyInjection;
using System;
using tusdotnet;
using XtraUpload.StorageManager.Service;
using XtraUpload.StorageManager.Common;
using XtraUpload.Domain.Infra;

namespace XtraUpload.StorageManager.Host
{
    public static class StartUp
    {
        public static void UseStorageManager(this IApplicationBuilder app)
        {
            app.UseTus(httpContext =>
            {
                if (httpContext.Request.Path.StartsWithSegments(new PathString("/avatarupload")))
                {
                    return httpContext.RequestServices.GetService<AvatarUploadService>().CreateTusConfiguration();
                }
                else if (httpContext.Request.Path.StartsWithSegments(new PathString("/fileupload")))
                {
                    return httpContext.RequestServices.GetService<FileUploadService>().CreateTusConfiguration();
                }
                else return null;
            });
        }
        public static void AddStorageManager(this IServiceCollection services, IConfiguration config)
        {
            // Registre services
            services.AddSingleton<AvatarUploadService>();
            services.AddSingleton<FileUploadService>();
            services.AddImageSharp();

            
            // Background jobs
            services.AddHostedService<ExpiredFilesCleanupService>();
            
            // Health check
            services.AddHealthChecks().AddCheck<FileStoreHealthCheck>("Storage Permissions");

            // Upload Options
            IConfigurationSection uploadSection = config.GetSection(nameof(UploadOptions));
            services.Configure<UploadOptions>(uploadSection);
            services.ConfigureWritable<UploadOptions>(uploadSection);
        }
    }
}
