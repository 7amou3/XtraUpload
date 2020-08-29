using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using XtraUpload.FileManager.Service.Common;
using XtraUpload.Setting.Service.Common;

namespace XtraUpload.Setting.Host
{
    public class StorageHealthCheck : IHealthCheck
    {
        private readonly HardwareCheckOptions _hardwareOpts;
        private readonly UploadOptions _uploadOpts;

        public StorageHealthCheck(IOptionsMonitor<HardwareCheckOptions> hardwraeOpts, IOptionsMonitor<UploadOptions> uploadsOpts)
        {
            _hardwareOpts = hardwraeOpts.CurrentValue;
            _uploadOpts = uploadsOpts.CurrentValue;
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            double threshold = _hardwareOpts.StorageThreshold * (1024L * 1024L * 1024L); // x * 1 Gb

            string rootDrive = Path.GetPathRoot(_uploadOpts.UploadPath);
            DriveInfo driveInfo = DriveInfo.GetDrives().FirstOrDefault(s => s.Name == rootDrive);
            if (driveInfo != null)
            {
                double usedSpace = driveInfo.TotalSize - driveInfo.AvailableFreeSpace;
                string driveUsage = ((usedSpace / threshold) * 100).ToString("0.00");
                if (usedSpace > threshold)
                {
                    return Task.FromResult(HealthCheckResult.Unhealthy("Maximum application storage space has been reached!"));
                }
                else if ((usedSpace / threshold) > 0.85)
                {
                    return Task.FromResult(HealthCheckResult.Degraded($"Storage space: {driveUsage}%"));
                }
                else
                {
                    return Task.FromResult(HealthCheckResult.Healthy($"Storage space: {driveUsage}%"));
                }
            }
            else
            {
                return Task.FromResult(HealthCheckResult.Unhealthy($"No disck found with the name: {rootDrive}"));
            }
        }
    }
}
