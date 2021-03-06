using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MediatR;
using XtraUpload.Domain;
using XtraUpload.StorageManager.Host;

namespace XtraUpload.StorageServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // see https://docs.microsoft.com/en-us/aspnet/core/security/cors?view=aspnetcore-3.1 to configure cors according to your needs
            services.AddCors();
            services.AddControllers();
            services.AddStorageManager(Configuration);
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            // Add mediatr
            services.AddMediatR(typeof(Startup), typeof(StorageManager.Host.Startup));
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

            // setup api to work with proxy servers and load balancers
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseStaticFiles();

            app.UseRouting();

            app.UseCors(builder => builder
                                    .AllowAnyHeader()
                                    .AllowAnyMethod()
                                    .AllowAnyOrigin()
                                    .WithExposedHeaders(tusdotnet.Helpers.CorsHelper.GetExposedHeaders().Append("upload-data").ToArray()));

            app.UseStorageManager();
            
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

        }
    }
}
