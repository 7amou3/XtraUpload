using System;
using XtraUpload.Authentication.Service.Common;
using XtraUpload.Authentication.Service;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using XtraUpload.Domain;
using Microsoft.AspNetCore.Identity;
using XtraUpload.Database.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using MediatR;

namespace XtraUpload.Authentication.Host
{
    public static class Startup
    {
        static SymmetricSecurityKey _signingKey;
        public static void AddXtraUploadAuthentication(this IServiceCollection services, IConfiguration config)
        {
            // Registre services
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddSingleton<IJwtFactory, JwtFactory>();
            
            // Register jwt
            ConfigureJwt(services, config);

            // Deserialize jwt configs from appsettings and store them for DI
            RegisterOptions(services, config);

            // Add mediatr (no need to register all handlers, mediatr will scan the assembly and register them automatically)
            services.AddMediatR(typeof(StandardLoginQueryHandler));
        }
        /// <summary>
        /// Configure jwt settings
        /// </summary>
        private static void ConfigureJwt(IServiceCollection services, IConfiguration config)
        {
            var jwtAppSettingOptions = config.GetSection(nameof(JwtIssuerOptions));
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
            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(configureOptions =>
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
        /// Deserialize configs from appsettings and store them in DI
        /// </summary>
        private static void RegisterOptions(IServiceCollection services, IConfiguration config)
        {
            services.Configure<JwtIssuerOptions>(config.GetSection(nameof(JwtIssuerOptions)));
            services.Configure<JwtIssuerOptions>(options =>
            {
                options.SigningCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256);
            });

            // Social Auth settings 
            IConfigurationSection socialAuthSection = config.GetSection(nameof(SocialAuthSettings));
            services.Configure<SocialAuthSettings>(socialAuthSection);
        }
    }
}
