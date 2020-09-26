using System.Linq;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using XtraUpload.Administration.Service.Common;
using XtraUpload.Authentication.Service.Common;
using XtraUpload.Domain;
using XtraUpload.Domain.Infra;
using XtraUpload.FileManager.Service.Common;
using XtraUpload.WebApp.Filters;
using XtraUpload.Setting.Service.Common;
using Microsoft.AspNetCore.HttpOverrides;
using XtraUpload.Authentication.Host;
using XtraUpload.FileManager.Host;
using XtraUpload.Email.Host;
using XtraUpload.Administration.Host;
using XtraUpload.Setting.Host;
using XtraUpload.Database.Host;
using XtraUpload.gRPCServer;

namespace XtraUpload.WebApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddHttpClient();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // Add cors, see https://docs.microsoft.com/en-us/aspnet/core/security/cors?view=aspnetcore-3.1 to configure cors according to your needs
            services.AddCors();

            // Add grpc server
            services.AddGrpc();

            // Load XtraUpload modules
            services.AddDatabase(Configuration);
            services.AddXtraUploadAuthentication(Configuration);
            services.AddXtraUploadSetting(Configuration);
            services.AddFileManager();
            services.AddEmail(Configuration);
            services.AddAdministration();

            // Add background worker
            services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
            services.AddHostedService<QueuedHostedService>();

            // Add mediatr
            services.AddMediatR(
                typeof(Startup),
                typeof(Setting.Host.Startup),
                typeof(Authentication.Host.Startup));

            
            RegisterDto(services);

            // Unhandled exception filter handler
            services.AddScoped<ApiExceptionFilter>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddLog4Net();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            // setup api to work with proxy servers and load balancers
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseStaticFiles();

            app.UseAuthentication();
            
            app.UseRouting();

            app.UseAuthorization();

            app.UseCors(builder => builder
                                    .AllowAnyHeader()
                                    .AllowAnyMethod()
                                    .AllowAnyOrigin());

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapGrpcService<gFileStorageService>();
            });

            app.UseHealthChecks("/health", new HealthCheckOptions
            {
                ResponseWriter = async (context, report) =>
                {
                    context.Response.ContentType = "application/json";
                    var response = new HealthCheckResponse
                    {
                        Status = report.Status.ToString(),
                        Checks = report.Entries.Select(x => new HealthCheck
                        {
                            Component = x.Key,
                            Status = x.Value.Status.ToString(),
                            Description = x.Value.Description
                        }),
                        Duration = report.TotalDuration
                    };
                    await context.Response.WriteAsync(Helpers.JsonSerialize(response));
                }
            });

        }

        /// <summary>
        /// Register Dto
        /// </summary>
        /// <param name="services"></param>
        private void RegisterDto(IServiceCollection services)
        {
            services.AddAutoMapper(cfg => {
                cfg.CreateMap<User, StandardLoginQuery>();
                cfg.CreateMap<User, UserDto>();
                cfg.CreateMap<User, SearchUserDto>();
                cfg.CreateMap<Role, RoleDto>();
                cfg.CreateMap<RolesResult, RolesResultDto>();
                cfg.CreateMap<RoleClaimsResult, RoleClaimsResultDto>();
                cfg.CreateMap<RoleClaim, RoleClaimDto>();
                cfg.CreateMap<FolderItem, FolderViewModel>();
                cfg.CreateMap<FileItem, FileItemDto>();
                cfg.CreateMap<FileItem, FileItemHeaderDto>();
                cfg.CreateMap<UploadSettingResult, UploadSettingDto>();
                cfg.CreateMap<FilesStatsResult, FilesStatsDto>();
                cfg.CreateMap<AccountOverviewResult, AccountOverviewDto>();
                cfg.CreateMap<DownloadSettingResult, DownloadSettingDto>();
                cfg.CreateMap<SocialMediaUser, SocialMediaLoginQuery>();
                cfg.CreateMap<JwtIssuerOptions, JwtIssuerOptionsDto>();
                cfg.CreateMap<ReadAppSettingResult, ReadAppSettingResultDto>();
                cfg.CreateMap<DeleteFolderResult, DeleteFolderResultDto>();
                cfg.CreateMap<DeleteItemsResult, DeleteItemsResultDto>();
            },
            typeof(Startup));
        }
    }
}
