using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using tusdotnet.Interfaces;
using tusdotnet.Models;
using tusdotnet.Models.Expiration;
using XtraUpload.FileManager.Service;

namespace XtraUpload.WebApp
{
    /// <summary>
    /// A background job used to remove expired non completed/aborted files from the store
    /// </summary>
    public class ExpiredFilesCleanupService : IHostedService, IDisposable
    {
        private Timer _timer;
        private readonly ITusExpirationStore _expirationStore;
        private readonly ExpirationBase _expiration;
        private readonly ILogger<ExpiredFilesCleanupService> _logger;

        public ExpiredFilesCleanupService(FileUploadService fileUploadService, ILogger<ExpiredFilesCleanupService> logger)
        {
            _logger = logger;
            DefaultTusConfiguration config = fileUploadService.GetTusConfiguration();
            _expirationStore = (ITusExpirationStore)config.Store;
            _expiration = config.Expiration;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            if (_expiration == null)
            {
                _logger.LogInformation("Not running cleanup job as no expiration has been set.");
                return;
            }

            await RunCleanup(cancellationToken);
            _timer = new Timer(async (e) => await RunCleanup((CancellationToken)e), cancellationToken, TimeSpan.Zero, _expiration.Timeout);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        private async Task RunCleanup(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Running cleanup job...");
                int numberOfRemovedFiles = await _expirationStore.RemoveExpiredFilesAsync(cancellationToken);
                _logger.LogInformation($"Removed {numberOfRemovedFiles} expired files. Scheduled to run again in {_expiration.Timeout.TotalMilliseconds} ms");
            }
            catch (Exception exc)
            {
                _logger.LogWarning("Failed to run cleanup job: " + exc.Message);
            }
        }

        public void Dispose()
        {
            if (_timer != null)
            {
                _timer.Change(Timeout.Infinite, Timeout.Infinite);
                _timer.Dispose();
                _timer = null;
            }
        }
    }
}