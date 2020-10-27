using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using XtraUpload.GrpcServices.Common;

namespace XtraUpload.GrpcServices
{
    public class ClientCertificateValidator
    {
        RootCertificateConfig _certConfig;
        readonly ILogger<ClientCertificateValidator> _logger;
        public ClientCertificateValidator(IOptions<RootCertificateConfig> certConfig, ILogger<ClientCertificateValidator> logger)
        {
            _certConfig = certConfig.Value;
            _logger = logger;
        }

        /// <summary>
        /// Try to validate the incoming client certificate against the server root cert
        /// </summary>
        public Task IsValid(CertificateValidatedContext context)
        {
            bool isValid = false;
            if (! File.Exists(_certConfig.CrtPath))
            {
                return SetInvalidContext(context, "Invalid path, the public root certificate could not be found " + _certConfig.CrtPath);
            }

            using X509Certificate2 serverCert = new X509Certificate2(_certConfig.CrtPath);
            using X509Chain chain = new X509Chain();
            chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
            try
            {
                if (!chain.Build(context.ClientCertificate))
                {
                    return SetInvalidContext(context, "Building the client certificate has failed.");
                }

                // Validate only certificates that are issued by our private CA
                isValid = chain.ChainElements.Cast<X509ChainElement>().Any(x => x.Certificate.Thumbprint == serverCert.Thumbprint);
                if (!isValid)
                {
                    return SetInvalidContext(context, "Chain of trust certification failed.");
                }
                context.Success();
                return Task.CompletedTask;
            }
            catch (Exception _ex)
            {
                return SetInvalidContext(context, _ex.Message);
            }
        }

        private Task SetInvalidContext(CertificateValidatedContext context, string errorMsg)
        {
            context.Fail(new Exception(errorMsg));

            _logger.LogError(errorMsg);
            return Task.CompletedTask;
        }
    }
}
