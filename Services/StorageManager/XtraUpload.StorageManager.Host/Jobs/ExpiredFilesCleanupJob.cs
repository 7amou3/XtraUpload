using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using tusdotnet.Interfaces;
using tusdotnet.Models;
using tusdotnet.Models.Expiration;
using XtraUpload.StorageManager.Service;

namespace XtraUpload.StorageManager.Host
{
    /// <summary>
    /// A background job used to remove expired non completed/aborted files from the store
    /// </summary>
    public class ExpiredFilesCleanupJob : IHostedService, IDisposable
    {
        private Timer _timer;
        private readonly ITusExpirationStore _expirationStore;
        private readonly ExpirationBase _expiration;
        private readonly ILogger<ExpiredFilesCleanupJob> _logger;

        public ExpiredFilesCleanupJob(FileUploadService fileUploadService, ILogger<ExpiredFilesCleanupJob> logger)
        {
            _logger = logger;
            DefaultTusConfiguration config = fileUploadService.CreateTusConfiguration();
            _expirationStore = (ITusExpirationStore)config.Store;
            _expiration = config.Expiration;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            if (_expiration == null)
            {
                _logger.LogInformation("Not running cleanup job as no expiration has been set.");
            }
            else
            {
                _timer = new Timer(RunCleanup, cancellationToken, int.Parse(_expiration.Timeout.TotalMilliseconds.ToString()), Timeout.Infinite);
            }    
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        private async void RunCleanup(object state)
        {
            try
            {
                _logger.LogInformation("Running cleanup job...");
                int numberOfRemovedFiles = await _expirationStore.RemoveExpiredFilesAsync((CancellationToken)state);
                _logger.LogInformation($"Removed {numberOfRemovedFiles} expired files. Scheduled to run again in {_expiration.Timeout.TotalMilliseconds} ms");
            }
            catch (Exception exc)
            {
                _logger.LogWarning("Failed to run cleanup job: " + exc.Message);
            }
            finally
            {
                if (!((CancellationToken)state).IsCancellationRequested)
                {
                    // Re-schedule timer
                    _timer.Change(int.Parse(_expiration.Timeout.TotalMilliseconds.ToString()), Timeout.Infinite);
                }
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