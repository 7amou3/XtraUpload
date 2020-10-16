using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using XtraUpload.Domain;
using XtraUpload.GrpcServices.Common;

namespace XtraUpload.GrpcServices
{
    /// <summary>
    /// Singleton class used to interact with the connected storage clients, retrieve configuration,...
    /// Connexion type for a storage client is duplex
    /// </summary>
    public class StorageClientProxy : IStorageClientProxy
    {
        /// <summary>
        /// Max time to wait before request timed out (in miliseconds)
        /// </summary>
        private const ushort WAIT_TIME = 10000;
        /// <summary>
        /// Thread sync 
        /// </summary>
        private readonly ManualResetEventSlim _sync = new ManualResetEventSlim();
        /// <summary>
        /// This dictionay contains server address as key associated to it's upload config
        /// </summary>
        private Dictionary<string, UploadOptions> _serversConfig = new Dictionary<string, UploadOptions>();
        /// <summary>
        /// Logger instance
        /// </summary>
        private readonly ILogger<StorageClientProxy> _logger;

        /// <summary>
        /// Create new instance of <see cref="StorageClientProxy"/>
        /// </summary>
        public StorageClientProxy(ILogger<StorageClientProxy> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Event raised to make a request to the designated client storage
        /// </summary>
        public event EventHandler<UploadOptsRequestedEventArgs> UploadOptsRequested;

        /// <summary>
        /// Sets the upload options for a given server (must be called only in the GrpcServices project)
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
            _sync.Set();
        }

        /// <summary>
        /// Gets the <see cref="UploadOptions"/> for the given server
        /// </summary>
        public UploadOptionsResult GetUploadOptions(string serverAddress)
        {
            UploadOptionsResult Result = new UploadOptionsResult();
            // Raise event to call client storage server
            UploadOptsRequested?.Invoke(this, new UploadOptsRequestedEventArgs(serverAddress));
            if(_sync.Wait(WAIT_TIME))
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
                Result.ErrorContent = new ErrorContent("Timeout: Response not received, check that the server is up and running.", ErrorOrigin.Server);
            }

            return Result;
        }

    }
}
