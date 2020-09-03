using System;
using XtraUpload.Administration.Service;
using XtraUpload.Administration.Service.Common;
using Microsoft.Extensions.DependencyInjection;

namespace XtraUpload.Administration.Host
{
    public static class Startup
    {
        public static void AddAdministration(this IServiceCollection services)
        {
            services.AddScoped<IAdministrationService, AdministrationService>();
        }
    }
}
