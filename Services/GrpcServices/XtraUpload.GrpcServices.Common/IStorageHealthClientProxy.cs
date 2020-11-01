using System;
using System.Collections.Generic;

namespace XtraUpload.GrpcServices.Common
{
    public interface IStorageHealthClientProxy
    {
        /// <summary>
        /// Gets the current storage servers health status
        /// </summary>
        IEnumerable<StorageHealthResult> GetServersHealth { get; }
    }

    public interface IStorageHealthClientCommand
    {
        /// <summary>
        /// Event raised to read storage health
        /// </summary>
        event EventHandler<ReadStorageHealthEventArgs> ReadStorageHealthRequested;
        /// <summary>
        /// Initialize the service
        /// </summary>
        void Initialize();
        /// <summary>
        /// Sets the storage server health status
        /// </summary>
        void SetStorageHealthStatus(StorageHealthResult storageHealth, string callerAddress);
    }
}
