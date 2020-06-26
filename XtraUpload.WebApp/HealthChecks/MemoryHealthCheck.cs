using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;
using XtraUpload.WebApp.Common;

namespace XtraUpload.WebApp
{
    public class MemoryHealthCheck : IHealthCheck
    {
        private readonly HardwareCheckOptions _options;

        public MemoryHealthCheck(IOptionsMonitor<HardwareCheckOptions> options)
        {
            _options = options.CurrentValue;
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            // Include GC information in the reported diagnostics.
            double allocated = GC.GetTotalMemory(forceFullCollection: false) + GC.CollectionCount(0) + GC.CollectionCount(1) + GC.CollectionCount(2);
            double threshold = _options.MemoryThreshold * (1024L * 1024L * 1024L); // x * 1 Gb

            string memoryUsage = ((allocated / threshold) * 100).ToString("0.00");
            if (allocated > threshold)
            {
                return Task.FromResult(HealthCheckResult.Unhealthy("Maximum application RAM memory has been reached!"));
            }
            else if ((allocated / threshold) > 0.85)
            {
                return Task.FromResult(HealthCheckResult.Degraded($"Memory usage: {memoryUsage}%"));
            }
            else
            {
                return Task.FromResult(HealthCheckResult.Healthy($"Memory usage: {memoryUsage}%"));
            }
        }

    }
}
