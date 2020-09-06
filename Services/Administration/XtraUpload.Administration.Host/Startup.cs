using System;
using XtraUpload.Administration.Service;
using XtraUpload.Administration.Service.Common;
using Microsoft.Extensions.DependencyInjection;
using MediatR;

namespace XtraUpload.Administration.Host
{
    public static class Startup
    {
        public static void AddAdministration(this IServiceCollection services)
        {
            // Add mediatr (no need to register all handlers, mediatr will scan the assembly and register them automatically)
            services.AddMediatR(typeof(GetAdminOverViewQueryHandler));
        }
    }
}
