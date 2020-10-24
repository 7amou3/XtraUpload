using System.Linq;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using XtraUpload.Administration.Service.Common;
using XtraUpload.Authentication.Service.Common;
using XtraUpload.Domain;
using XtraUpload.Domain.Infra;
using XtraUpload.FileManager.Service.Common;
using XtraUpload.WebApi.Filters;
using XtraUpload.Setting.Service.Common;
using Microsoft.AspNetCore.HttpOverrides;
using XtraUpload.Authentication.Host;
using XtraUpload.FileManager.Host;
using XtraUpload.Email.Host;
using XtraUpload.Administration.Host;
using XtraUpload.Setting.Host;
using XtraUpload.Database.Host;
using XtraUpload.GrpcServices;
using Microsoft.AspNetCore.SpaServices.AngularCli;

namespace XtraUpload.WebApi
{
    public class Startup
    {
        /// <summary> Client application directory </summary>
        private readonly string _clientAppDir = "AngularApp";

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

            // Load XtraUpload modules
            services.AddDatabase(Configuration);
            services.AddXtraUploadAuthentication(Configuration);
            services.AddXtraUploadSetting(Configuration);
            services.AddFileManager();
            services.AddEmail(Configuration);
            services.AddAdministration();
            services.AddGrpcServices();

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

            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = _clientAppDir;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddLog4Net();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseSpaStaticFiles(new StaticFileOptions
                {
                    OnPrepareResponse = ctx =>
                    {
                        ctx.Context.Response.Headers.Add("Cache-Control", "max-age=86400");
                    }
                });
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
            });

            app.UseGrpcServices();

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

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = _clientAppDir;
                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
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
                cfg.CreateMap<FolderItem, FolderItemDto>();
                cfg.CreateMap<StorageServer, StorageServerDto>();
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
                cfg.CreateMap<GetFolderContentResult, FolderContentDto>();
            },
            typeof(Startup));
        }
    }
}
