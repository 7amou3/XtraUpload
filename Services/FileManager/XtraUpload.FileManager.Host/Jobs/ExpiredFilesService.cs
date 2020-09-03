using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using XtraUpload.Database.Data.Common;
using XtraUpload.Domain;
using XtraUpload.FileManager.Service.Common;

namespace XtraUpload.FileManager.Host
{
    /// <summary>
    /// A background job used to remove expired user files from the store
    /// An expired file is a file that did not receive a download for x day
    /// </summary>
    public class ExpiredFilesService : IHostedService, IDisposable
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ExpiredFilesService> _logger;
        private readonly int _dueTime = 3600 * 1000; // 1 hour
        readonly UploadOptions _uploadOpt;
        private Timer _timer;

        public ExpiredFilesService(IServiceProvider serviceProvider, IOptionsMonitor<UploadOptions> uploadOpt, ILogger<ExpiredFilesService> logger)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _uploadOpt = uploadOpt.CurrentValue;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(RunCleanup, cancellationToken, _dueTime, Timeout.Infinite);
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

                using IServiceScope scope = _serviceProvider.CreateScope();
                using IUnitOfWork unitOfWork = scope.ServiceProvider.GetService<IUnitOfWork>();
                IEnumerable<FileItem> files = await unitOfWork.Files.GetExpiredFiles((CancellationToken)state);
                
                // Remove the files from the drive
                foreach (var file in files)
                {
                    string folderPath = Path.Combine(_uploadOpt.UploadPath, file.UserId, file.Id);

                    if (Directory.Exists(folderPath))
                    {
                        Directory.Delete(folderPath, true);
                    }
                }

                // Remove the files from Db
                unitOfWork.Files.RemoveRange(files);
                await unitOfWork.CompleteAsync();

                _logger.LogInformation($"Removed {files.Count()} expired files. Scheduled to run again in {_dueTime / 1000 / 60} minutes");
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Failed to run cleanup job: " + ex.Message);
            }
            finally
            {
                if ( ! ((CancellationToken)state).IsCancellationRequested)
                {
                    // Re-schedule timer
                    _timer.Change(_dueTime, Timeout.Infinite);
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
