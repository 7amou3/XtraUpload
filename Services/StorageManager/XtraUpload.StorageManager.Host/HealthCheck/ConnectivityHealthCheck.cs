using Grpc.Core;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XtraUpload.Protos;

namespace XtraUpload.StorageManager.Host
{
    /// <summary>
    /// Check if this storage server is connected to the grpc server
    /// </summary>
    class ConnectivityHealthCheck : IHealthCheck
    {
        gStorageManager.gStorageManagerClient _storageClient;
        public ConnectivityHealthCheck(gStorageManager.gStorageManagerClient storageClient)
        {
            _storageClient = storageClient;
        }
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _storageClient.IsAuthorizedAsync(new Google.Protobuf.WellKnownTypes.Empty());
                if (response != null && response.Status != null && response.Status.Status == RequestStatus.Success)
                {
                    HealthCheckResult.Healthy();
                }
                else
                {
                    return HealthCheckResult.Unhealthy("No connection could be made because the target machine actively refused it.");
                }
            }
            catch (Exception _ex)
            {
                return HealthCheckResult.Unhealthy(_ex.Message);
            }
            return HealthCheckResult.Healthy();
        }
    }
}
