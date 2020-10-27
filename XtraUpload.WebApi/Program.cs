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
                    webBuilder.ConfigureKestrel(kestrelOptions =>
                    {
                        // 5000 is the port for the web API
                        kestrelOptions.ListenLocalhost(5000, listenOptions =>
                        {
                            listenOptions.UseHttps();
                        });
                        // 5001 is the port for grpc service
                        kestrelOptions.ListenLocalhost(5001,
                        listenOptions =>
                        {
                            listenOptions.UseHttps(options => 
                            {
                                options.ClientCertificateMode = ClientCertificateMode.RequireCertificate; 
                            });
                        });

                    });
                    webBuilder.UseStartup<Startup>();
                });
    }
}
