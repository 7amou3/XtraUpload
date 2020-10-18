using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XtraUpload.Domain;

namespace XtraUpload.GrpcServices.Common
{
    public interface ICheckClientProxy
    {
        /// <summary>
        /// Event raised to check the client storage connectivity
        /// </summary>
        event EventHandler<StorageServerConnectivityEventArgs> StorageServerConnectivityRequested;

        /// <summary>
        /// Check the storage server connectivity
        /// </summary>
        Task<StorageServerConnectivityResult> CheckServerStorageConnectivity(string serverAddress);
        /// <summary>
        /// Sets the connectivity status of the given storage server (must be called only in the GrpcServices project)
        /// </summary>
        void SetConnectivityStatus(OperationResult result);
    }
}
