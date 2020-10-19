using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
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
        public static void AddGrpcServices(this IServiceCollection services)
        {
            // Add grpc server
            services.AddGrpc();
            // Register services
            services.AddSingleton<ICheckClientProxy, CheckClientProxy>();
            services.AddSingleton<IUploadOptsClientProxy, UploadOptsClientProxy>();
            services.AddSingleton<IHardwareOptsClientProxy, HardwareOptsClientProxy>();
        }
    }
}
