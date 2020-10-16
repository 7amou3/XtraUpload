using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SixLabors.ImageSharp.Web.DependencyInjection;
using System;
using tusdotnet;
using XtraUpload.StorageManager.Service;
using XtraUpload.Domain;
using XtraUpload.Domain.Infra;
using XtraUpload.Protos;
using MediatR;

namespace XtraUpload.StorageManager.Host
{
    public static class Startup
    {
        public async static void UseStorageManager(this IApplicationBuilder app)
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
            await app.ApplicationServices.GetService<StartDuplexClient>().Start();
        }
        public static void AddStorageManager(this IServiceCollection services, IConfiguration config)
        {
            // Registre services
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<AvatarUploadService>();
            services.AddSingleton<FileUploadService>();
            services.AddSingleton<LoggerInterceptor>();
            
            services.AddSingleton<StartDuplexClient>();
            services.AddImageSharp();

            // Add grpc clients
            services.AddGrpcClient<gFileStorage.gFileStorageClient>(options =>
            {
                options.Address = new Uri(config["ApiUrl"]);
            })
            .ConfigureChannel((serviceProvider, channel) =>
            {
                channel.Credentials = GrpcChannelHelper.CreateSecureChannel(serviceProvider);
            })
            .AddInterceptor<LoggerInterceptor>();

            services.AddGrpcClient<gStorageManager.gStorageManagerClient>(options =>
            {
                options.Address = new Uri(config["ApiUrl"]);
            })
            .ConfigureChannel((serviceProvider, channel) =>
            {
                channel.Credentials = GrpcChannelHelper.CreateSecureChannel(serviceProvider);
            })
            .AddInterceptor<LoggerInterceptor>();

            // Add mediatr (no need to register all handlers, mediatr will scan the assembly and register them automatically)
            services.AddMediatR(typeof(GetThumbnailQueryHandler));

            // Background jobs
            services.AddHostedService<ExpiredFilesCleanupJob>();
            services.AddHostedService<DeleteFilesJob>();
            
            // Health check
            services.AddHealthChecks()
                .AddCheck<FileStoreHealthCheck>("Storage Permissions")
                .AddCheck<StorageHealthCheck>("Storage Space");

            // Upload Options
            IConfigurationSection uploadSection = config.GetSection(nameof(UploadOptions));
            services.Configure<UploadOptions>(uploadSection);
            services.ConfigureWritable<UploadOptions>(uploadSection);
        }
    }
}
