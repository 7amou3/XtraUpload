using tusdotnet;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using XtraUpload.FileManager.Service;
using XtraUpload.FileManager.Service.Common;
using SixLabors.ImageSharp.Web.DependencyInjection;
using XtraUpload.Domain.Infra;
using MediatR;

namespace XtraUpload.FileManager.Host
{
    public static class Startup
    {
        public static void UseFileManager(this IApplicationBuilder app)
        {
            // Uncomment if you want to serve the angular app from other domain or to allow 3rd paries to query XtraUpload's API
            /*app.UseCors(builder => builder
                                    .AllowAnyHeader()
                                    .AllowAnyMethod()
                                    .AllowAnyOrigin()
                                    .WithExposedHeaders(tusdotnet.Helpers.CorsHelper.GetExposedHeaders()));*/

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
        public static void AddFileManager(this IServiceCollection services, IConfiguration config)
        {
            // Registre services
            services.AddSingleton<AvatarUploadService>();
            services.AddSingleton<FileUploadService>();
            services.AddImageSharp();

            // Add mediatr (no need to register all handlers, mediatr will scan the assembly and register them automatically)
            services.AddMediatR(typeof(CreateFolderCommandHandler));

            // Background jobs
            services.AddHostedService<ExpiredFilesCleanupService>();
            services.AddHostedService<ExpiredFilesService>();

            // Health check
            services.AddHealthChecks().AddCheck<FileStoreHealthCheck>("Storage Permissions");

            // Upload Options
            IConfigurationSection uploadSection = config.GetSection(nameof(UploadOptions));
            services.Configure<UploadOptions>(uploadSection);
            services.ConfigureWritable<UploadOptions>(uploadSection);
        }
    }
}
