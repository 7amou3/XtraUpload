using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace XtraUpload.Services.Test
{
    public static class TestServerFactory
    {
        public static TestServer Create(Action<IApplicationBuilder> startup, Action<IServiceCollection> configureServices)
        {
            var host = new WebHostBuilder().Configure(startup).ConfigureServices(configureServices);
            return new TestServer(host);
        }
    }
}
