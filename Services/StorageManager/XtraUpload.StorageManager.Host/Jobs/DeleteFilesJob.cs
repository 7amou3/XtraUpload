using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XtraUpload.StorageManager.Common;

namespace XtraUpload.StorageManager.Host
{
    public class DeleteFilesJob : IHostedService, IDisposable
    {
        private const int EXPIRATION = 60 * 60 * 1000; // 1 hour
        private Timer _timer;
        private readonly IMediator _mediatr;
        private readonly ILogger<DeleteFilesJob> _logger;

        public DeleteFilesJob(IMediator mediatr, ILogger<DeleteFilesJob> logger)
        {
            _mediatr = mediatr;
            _logger = logger;
            
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(RunCleanup, cancellationToken, EXPIRATION, Timeout.Infinite);

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
                
                var result = await _mediatr.Send(new DeleteFilesCommand());
                if (result.State == Domain.OperationState.Success)
                {
                    _logger.LogInformation($"Removed {result.Files.Count()} taged files. Scheduled to run again in {EXPIRATION} ms");
                }
                else
                {
                    _logger.LogError("Error occured while deleting files from disk " + result.ErrorContent.Message);
                }
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
                    _timer.Change(EXPIRATION, Timeout.Infinite);
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
