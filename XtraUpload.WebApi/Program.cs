using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.Extensions.Hosting;

namespace XtraUpload.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureKestrel((context, serverOptions) =>
                    {
                        // Require client cert for gRPC server endpoint
                        serverOptions.Configure(context.Configuration.GetSection("Kestrel"))
                        .Endpoint("gRPCServer", listenOptions =>
                        {
                            listenOptions.HttpsOptions.ClientCertificateMode = ClientCertificateMode.AllowCertificate;
                        });
                    });
                    webBuilder.UseStartup<Startup>();
                });
    }
}
