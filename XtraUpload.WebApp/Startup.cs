using System;
using System.Linq;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using SixLabors.ImageSharp.Web.DependencyInjection;
using tusdotnet;
using XtraUpload.Administration.Service;
using XtraUpload.Administration.Service.Common;
using XtraUpload.Authentication.Service;
using XtraUpload.Authentication.Service.Common;
using XtraUpload.Database.Data;
using XtraUpload.Database.Data.Common;
using XtraUpload.Domain;
using XtraUpload.Domain.Infra;
using XtraUpload.Email.Service;
using XtraUpload.Email.Service.Common;
using XtraUpload.FileManager.Service;
using XtraUpload.FileManager.Service.Common;
using XtraUpload.WebApp.Common;
using XtraUpload.WebApp.Filters;
using XtraUpload.Setting.Service;
using XtraUpload.Setting.Service.Common;
using Microsoft.AspNetCore.HttpOverrides;

namespace XtraUpload.WebApp
{
    public class Startup
    {
        private SymmetricSecurityKey _signingKey;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // services.AddCors();
            services.AddControllers();
            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });
            services.AddImageSharp();
            RegisterOptions(services);
            RegisterDb(services);
            RegisterServices(services);
            RegisterDto(services);
            RegisterJwt(services);
            services.AddHttpClient();

            services.AddHealthChecks()
                .AddDbContextCheck<ApplicationDbContext>("Database Server")
                .AddCheck<MemoryHealthCheck>("Memory")
                .AddCheck<StorageHealthCheck>("Storage Space")
                .AddCheck<FileStoreHealthCheck>("Storage Permissions")
                .AddCheck<EmailService>("Email Service");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddLog4Net();

            app.Use((context, next) =>
            {
                context.Features.Get<IHttpMaxRequestBodySizeFeature>().MaxRequestBodySize = null;
                return next.Invoke();
            });
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseStaticFiles();

            if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles(new StaticFileOptions
                {
                    OnPrepareResponse = ctx =>
                    {
                        ctx.Context.Response.Headers.Add("Cache-Control", "max-age=86400");
                    }
                });
            }

            app.UseAuthentication();

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

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
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

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";
                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });
        }

        /// <summary>
        /// Deserialize configs from appsettings and store them in DI
        /// </summary>
        private void RegisterOptions(IServiceCollection services)
        {
            // Jwt Options
            IConfigurationSection jwtSection = Configuration.GetSection(nameof(JwtIssuerOptions));
            services.Configure<JwtIssuerOptions>(jwtSection);
            services.Configure<JwtIssuerOptions>(options =>
            {
                options.SigningCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256);
            });
            services.ConfigureWritable<JwtIssuerOptions>(Configuration.GetSection(nameof(JwtIssuerOptions)));

            // WebApp settings
            IConfigurationSection WebAppSection = Configuration.GetSection(nameof(WebAppSettings));
            services.Configure<WebAppSettings>(WebAppSection);
            services.ConfigureWritable<WebAppSettings>(Configuration.GetSection(nameof(WebAppSettings)));

            // Social Auth settings 
            IConfigurationSection socialAuthSection = Configuration.GetSection(nameof(SocialAuthSettings));
            services.Configure<SocialAuthSettings>(socialAuthSection);
            services.ConfigureWritable<SocialAuthSettings>(Configuration.GetSection(nameof(SocialAuthSettings)));

            // Upload Options
            IConfigurationSection uploadSection = Configuration.GetSection(nameof(UploadOptions));
            services.Configure<UploadOptions>(uploadSection);
            services.ConfigureWritable<UploadOptions>(Configuration.GetSection(nameof(UploadOptions)));

            // Email Configs
            IConfigurationSection emailSection = Configuration.GetSection(nameof(EmailSettings));
            services.Configure<EmailSettings>(emailSection);
            services.ConfigureWritable<EmailSettings>(Configuration.GetSection(nameof(EmailSettings)));

            // Hardware Options
            IConfigurationSection hardwareSection = Configuration.GetSection(nameof(HardwareCheckOptions));
            services.Configure<HardwareCheckOptions>(hardwareSection);
            services.ConfigureWritable<HardwareCheckOptions>(Configuration.GetSection(nameof(HardwareCheckOptions)));

        }

        private void RegisterJwt(IServiceCollection services)
        {
            var jwtAppSettingOptions = Configuration.GetSection(nameof(JwtIssuerOptions));
            _signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtAppSettingOptions[nameof(JwtIssuerOptions.SecretKey)]));
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)],

                ValidateAudience = true,
                ValidAudience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)],

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _signingKey,

                RequireExpirationTime = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(configureOptions =>
            {
                configureOptions.ClaimsIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)];
                configureOptions.TokenValidationParameters = tokenValidationParameters;
                configureOptions.SaveToken = true;
            });

            // add base claims
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Admin", policy => policy.RequireClaim(XtraUploadClaims.AdminAreaAccess.ToString(), "1"));
                options.AddPolicy("User", policy => policy.RequireClaim(XtraUploadClaims.FileManagerAccess.ToString()));
            });

            // add identity
            var builder = services.AddIdentityCore<User>(o =>
            {
                o.Password.RequireDigit = false;
                o.Password.RequireLowercase = false;
                o.Password.RequireUppercase = false;
                o.Password.RequireNonAlphanumeric = false;
                o.Password.RequiredLength = 6;
            });
            builder = new IdentityBuilder(builder.UserType, typeof(Role), builder.Services);
            builder.AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

        }

        /// <summary>
        /// Register Db provider against DI 
        /// </summary>
        private void RegisterDb(IServiceCollection services)
        {
            string dbProvider = Configuration["Database:Provider"].ToLower();
            if (string.CompareOrdinal(dbProvider, "sqlserver") == 0)
            {
                services.AddDbContext<ApplicationDbContext>(options =>
                   options.UseSqlServer(
                       Configuration["Database:ConnectionString"],
                       sqlServerOptions => sqlServerOptions.MigrationsAssembly("XtraUpload.Database.Migrations")));
            }
            else if (string.CompareOrdinal(dbProvider, "mysql") == 0)
            {
                services.AddDbContext<ApplicationDbContext>(options =>
                   options.UseMySql(
                       Configuration["DataBase:ConnectionString"],
                       mySqlServerOptions => mySqlServerOptions.MigrationsAssembly("XtraUpload.Database.Migrations")));
            }
            else
            {
                throw new Exception("Invalid Database provider, XtraUpload support: Sql or MySql as a provider.");
            }
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IRoleClaimsRepository, RoleClaimsRepository>();
            services.AddScoped<IFolderRepository, FolderRepository>();
            services.AddScoped<IFileRepository, FileRepository>();
            services.AddScoped<IDownloadRepository, DownloadRepository>();
            services.AddScoped<IConfirmationKeyRepository, ConfirmationKeyRepository>();
            services.AddScoped<IFileExtensionRepository, FileExtensionRepository>();
            services.AddScoped<IPageRepository, PageRepository>();
        }
        /// <summary>
        /// Register XtraUpload services against DI 
        /// </summary>
        private void RegisterServices(IServiceCollection services)
        {
            services.AddScoped<ApiExceptionFilter>();
            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IFileManagerService, FileManagerService>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IFileDownloadService, FileDownloadService>();
            services.AddScoped<ISettingService, SettingService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IAdministrationService, AdministrationService>();
            services.AddScoped<IAppSettingsService, AppSettingsService>();
            services.AddSingleton<IJwtFactory, JwtFactory>();
            services.AddSingleton<AvatarUploadService>();
            services.AddSingleton<FileUploadService>();
            services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
            services.AddHostedService<QueuedHostedService>();
            services.AddHostedService<ExpiredFilesCleanupService>();
            services.AddHostedService<ExpiredFilesService>();
        }

        /// <summary>
        /// Register Dto
        /// </summary>
        /// <param name="services"></param>
        private void RegisterDto(IServiceCollection services)
        {
            services.AddAutoMapper(cfg => {
                cfg.CreateMap<User, RegistrationViewModel>();
                cfg.CreateMap<User, CredentialsViewModel>();
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
                cfg.CreateMap<SocialMediaUser, SocialMediaLoginViewModel>();
                cfg.CreateMap<JwtIssuerOptions, JwtIssuerOptionsDto>();
                cfg.CreateMap<ReadAppSettingResult, ReadAppSettingResultDto>();
                cfg.CreateMap<DeleteFolderResult, DeleteFolderResultDto>();
                cfg.CreateMap<DeleteItemsResult, DeleteItemsResultDto>();
            },
            typeof(Startup));
        }
    }
}
