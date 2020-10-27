using Grpc.Core;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using XtraUpload.StorageManager.Common;

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

        /// <summary>
        /// Create a custom http handler by attaching a self signed cert to outgoing http requests
        /// </summary>
        public static HttpClientHandler CreateHttpHandler(IServiceProvider serviceProvider)
        {
            var handler = new HttpClientHandler();
            var clientCert = LoadSSLCertificate(serviceProvider);
            if (clientCert != null)
            {
                handler.ClientCertificates.Add(clientCert);
            }
            return handler;
        }

        private static X509Certificate2 LoadSSLCertificate(IServiceProvider serviceProvider)
        {
            var config = serviceProvider.GetService<IConfiguration>();
            var logger = serviceProvider.GetService<ILogger<GrpcChannelHelper>>();

            X509Certificate2 clientCertificate = null;
            try
            {
                var certConfig = config.GetSection(nameof(ClientCertificateConfig)).Get<ClientCertificateConfig>();
                if (File.Exists(certConfig.PfxPath))
                {
                    clientCertificate = new X509Certificate2(certConfig.PfxPath, certConfig.Password);
                }
                else
                {
                    logger.LogError("Certificate does not exist: " + certConfig.PfxPath + ", check that the provided path is valid.");
                }
            }
            catch (Exception _ex)
            {
                logger.LogError("Error occured while reading the certificate: " + _ex.Message);
            }
            return clientCertificate;
        }

    }
}
