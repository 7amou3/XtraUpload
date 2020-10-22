using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using XtraUpload.Domain;
using XtraUpload.GrpcServices.Common;

namespace XtraUpload.GrpcServices
{
    /// <summary>
    /// Singleton class used to interact with the connected storage clients to retrive upload configuration
    /// </summary>
    public class UploadOptsClientProxy : IUploadOptsClientProxy, IUploadOptsClientCommand
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
        /// This dictionay contains server address as key associated to it's upload config
        /// </summary>
        private Dictionary<string, UploadOptions> _serversConfig = new Dictionary<string, UploadOptions>();
        /// <summary>
        /// Logger instance
        /// </summary>
        private readonly ILogger<UploadOptsClientProxy> _logger;

        /// <summary>
        /// Create new instance of <see cref="UploadOptsClientProxy"/>
        /// </summary>
        public UploadOptsClientProxy(ILogger<UploadOptsClientProxy> logger)
        {
            _logger = logger;
        }

        #region IUploadOptsClientCommand members
        /// <summary>
        /// Event raised to read upload options of designated client storage
        /// </summary>
        public event EventHandler<ReadUploadOptsRequestedEventArgs> ReadUploadOptsRequested;
        /// <summary>
        /// Event raised to write upload options to designated client storage
        /// </summary>
        public event EventHandler<WriteUploadOptsRequestedEventArgs> WriteUploadOptsRequested;
        /// <summary>
        /// Sets the upload options for a given server
        /// </summary>
        public void SetUploadOptions(UploadOptions options, string callerAddress)
        {
            if (_serversConfig.TryGetValue(callerAddress, out UploadOptions opts))
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
        #endregion

        #region IUploadOptsClientProxy members

        /// <summary>
        /// Gets the <see cref="UploadOptions"/> for the given server
        /// </summary>
        public async Task<UploadOptionsResult> ReadUploadOptions(string serverAddress)
        {
            UploadOptionsResult Result = new UploadOptionsResult();
            // Raise event to call client storage server
            ReadUploadOptsRequested?.Invoke(this, new ReadUploadOptsRequestedEventArgs(serverAddress));
            if(await _signal.WaitAsync(WAIT_TIME))
            {
                if (_serversConfig.TryGetValue(serverAddress, out UploadOptions options))
                {
                    Result.UploadOpts = options;
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

        /// <summary>
        /// Sets the <see cref="UploadOptions"/> for the given server
        /// </summary>
        public async Task<OperationResult> WriteUploadOptions(UploadOptions uploadOpts, string serverAddress)
        {
            OperationResult Result = new OperationResult();
            WriteUploadOptsRequested?.Invoke(this, new WriteUploadOptsRequestedEventArgs(uploadOpts, serverAddress));
            if (await _signal.WaitAsync(WAIT_TIME))
            {
                if (!_serversConfig.TryGetValue(serverAddress, out UploadOptions options))
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

        #endregion
    }
}
