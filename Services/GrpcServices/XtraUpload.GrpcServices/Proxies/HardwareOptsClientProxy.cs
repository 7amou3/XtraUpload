using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XtraUpload.Domain;
using XtraUpload.GrpcServices.Common;

namespace XtraUpload.GrpcServices
{
    /// <summary>
    /// Singleton class used to interact with the connected storage clients to retrieve hardware configuration
    /// </summary>
    public class HardwareOptsClientProxy : IHardwareOptsClientProxy
    {
        /// <summary>
        /// Max time to wait before request timed out (in miliseconds)
        /// </summary>
        private const ushort WAIT_TIME = 10000;
        /// <summary>
        /// Thread sync 
        /// </summary>
        private readonly SemaphoreSlim _signal = new SemaphoreSlim(0, 1);
        /// <summary>
        /// This dictionay contains server address as key associated to it's hardware config
        /// </summary>
        private Dictionary<string, HardwareCheckOptions> _serversConfig = new Dictionary<string, HardwareCheckOptions>();
        /// <summary>
        /// Logger instance
        /// </summary>
        private readonly ILogger<UploadOptsClientProxy> _logger;

        /// <summary>
        /// Create new instance of <see cref="UploadOptsClientProxy"/>
        /// </summary>
        public HardwareOptsClientProxy(ILogger<UploadOptsClientProxy> logger)
        {
            _logger = logger;
        }
        /// <summary>
        /// Event raised to read hardware options configuration of the designated client storage
        /// </summary>
        public event EventHandler<HardwareOptsRequestedEventArgs> HardwareOptionsRequested;

        /// <summary>
        /// Sets the hardware options for a given server (must be called only in the GrpcServices project)
        /// </summary>
        public void SetHardwareOptions(HardwareCheckOptions options, string callerAddress)
        {
            if (_serversConfig.TryGetValue(callerAddress, out HardwareCheckOptions opts))
            {
                // update entry
                _serversConfig[callerAddress] = options;
            }
            else
            {
                // create new entry
                _serversConfig.TryAdd(callerAddress, options);
            }
            _signal.Release();
        }

        /// <summary>
        /// Gets the <see cref="HardwareCheckOptions"/> for the given server
        /// </summary>
        public async Task<HardwareCheckOptionsResult> GetHardwareOptions(string serverAddress)
        {
            HardwareCheckOptionsResult Result = new HardwareCheckOptionsResult();

            HardwareOptionsRequested?.Invoke(this, new HardwareOptsRequestedEventArgs(serverAddress));
            if (await _signal.WaitAsync(WAIT_TIME))
            {
                if (_serversConfig.TryGetValue(serverAddress, out HardwareCheckOptions options))
                {
                    Result.HardwareOptions = options;
                }
                else
                {
                    _logger.LogError("No record found for the server: " + serverAddress);
                    Result.ErrorContent = new ErrorContent("No record found, please try again.", ErrorOrigin.Server);
                }
            }
            else
            {
                _logger.LogError("Response timed out for server: " + serverAddress);
                Result.ErrorContent = new ErrorContent("Timeout: Response not received, please check that the server is up and running.", ErrorOrigin.Server);
            }

            return Result;
        }
    }
}
