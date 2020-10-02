using Grpc.Core;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace XtraUpload.StorageManager.Host
{
    public class GrpcChannelHelper
    {
        /// <summary>
        /// Create a secure grpc channel by attaching the jwt auth token to any outgoing grpc request
        /// </summary>
        public static ChannelCredentials CreateSecureChannel(IServiceProvider serviceProvider)
        {
            var credentials = CallCredentials.FromInterceptor((context, metadata) =>
            {
                var _httpContext = serviceProvider.GetService<IHttpContextAccessor>().HttpContext;
                if (_httpContext != null 
                    && _httpContext.Request.Headers.Any()
                    && !string.IsNullOrEmpty( _httpContext.Request.Headers["Authorization"]))
                {
                    metadata.Add("Authorization", _httpContext.Request.Headers["Authorization"]);
                }
                return Task.CompletedTask;
            });

            return ChannelCredentials.Create(new SslCredentials(), credentials);
        }
    }
}
