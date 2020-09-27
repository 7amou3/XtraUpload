using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace XtraUpload.GrpcServices
{
    public static class Startup
    {
        public static void UseGrpcServices(this IApplicationBuilder app)
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<gFileStorageService>();
            });
        }
        public static void AddGrpcServices(this IServiceCollection services)
        {
            // Add grpc server
            services.AddGrpc();
        }
    }
}
