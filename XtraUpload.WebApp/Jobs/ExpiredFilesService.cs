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
using XtraUpload.ServerApp.Common;

namespace XtraUpload.ServerApp
{
    /// <summary>
    /// A background job used to remove expired user files from the store
    /// An expired file is a file that did not receive a download for x day
    /// </summary>
    public class ExpiredFilesService : IHostedService, IDisposable
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ExpiredFilesService> _logger;
        readonly UploadOptions _uploadOpt;
        private Timer _timer;
        private TimeSpan _timeout = new TimeSpan(1, 0, 0);
        public ExpiredFilesService(IServiceProvider serviceProvider, IOptionsMonitor<UploadOptions> uploadOpt, ILogger<ExpiredFilesService> logger)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _uploadOpt = uploadOpt.CurrentValue;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await RunCleanup(cancellationToken);
            _timer = new Timer(async (e) => await RunCleanup((CancellationToken)e), cancellationToken, TimeSpan.Zero, _timeout);
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

                using IServiceScope scope = _serviceProvider.CreateScope();
                IUnitOfWork unitOfWork = scope.ServiceProvider.GetService<IUnitOfWork>();
                IEnumerable<FileItem> files = await unitOfWork.Files.GetExpiredFiles();
                
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

                _logger.LogInformation($"Removed {files.Count()} expired files. Scheduled to run again in {_timeout.TotalMinutes} minutes");
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
