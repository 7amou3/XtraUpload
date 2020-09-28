using Microsoft.Extensions.DependencyInjection;
using XtraUpload.FileManager.Service;
using MediatR;

namespace XtraUpload.FileManager.Host
{
    public static class Startup
    {
        public static void AddFileManager(this IServiceCollection services)
        {
            // Add mediatr (no need to register all handlers, mediatr will scan the assembly and register them automatically)
            services.AddMediatR(typeof(CreateFolderCommandHandler));

            // Background jobs
            services.AddHostedService<ExpiredFilesService>();
        }
    }
}
