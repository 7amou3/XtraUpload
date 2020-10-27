using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Cryptography.X509Certificates;
using XtraUpload.GrpcServices.Common;

namespace XtraUpload.GrpcServices
{
    public static class Startup
    {
        public static void UseGrpcServices(this IApplicationBuilder app)
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<gFileManagerService>();
                endpoints.MapGrpcService<gStorageManagerService>();
            });
        }
        public static void AddGrpcServices(this IServiceCollection services, IConfiguration config)
        {
            // Add grpc server
            services.AddGrpc();
            services.AddAuthentication(CertificateAuthenticationDefaults.AuthenticationScheme)
                .AddCertificate(options =>
                {
                    // The server is using a self-signed certificate, no need to check revocationMode.
                    options.RevocationMode = X509RevocationMode.NoCheck;
                    options.AllowedCertificateTypes = CertificateTypes.All;
                    
                    options.Events = new CertificateAuthenticationEvents()
                    {
                        OnCertificateValidated = context =>
                        {
                            var certValidator = context.HttpContext.RequestServices.GetService<ClientCertificateValidator>();
                            return certValidator.IsValid(context);
                        }
                    };
                });
            // Register services
            services.AddSingleton<ClientCertificateValidator>();
            services.AddSingleton<ICheckClientProxy, CheckClientProxy>();
            services.AddSingleton<IUploadOptsClientProxy, UploadOptsClientProxy>();
            services.AddSingleton<IHardwareOptsClientProxy, HardwareOptsClientProxy>();

            // Root certificate config
            IConfigurationSection certSection = config.GetSection(nameof(RootCertificateConfig));
            services.Configure<RootCertificateConfig>(certSection);
        }
    }
}
