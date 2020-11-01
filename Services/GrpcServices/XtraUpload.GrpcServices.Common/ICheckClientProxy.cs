using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using XtraUpload.Domain;

namespace XtraUpload.GrpcServices.Common
{
    public interface ICheckClientProxy
    {
        /// <summary>
        /// Check the storage server connectivity
        /// </summary>
        Task<StorageServerConnectivityResult> CheckServerStorageConnectivity(string serverAddress);
        /// <summary>
        /// A list of healthy servers
        /// </summary>
        IEnumerable<StorageServer> GetHealthyServers { get; }
    }

    public interface ICheckClientCommand
    {
        /// <summary>
        /// Event raised to check the client storage connectivity
        /// </summary>
        event EventHandler<StorageServerConnectivityEventArgs> StorageServerConnectivityRequested;
        /// <summary>
        /// Initialize the service
        /// </summary>
        void Initialize();
        /// <summary>
        /// Sets the connectivity status of the given storage server
        /// </summary>
        void SetConnectivityStatus(OperationResult result);
    }
}
