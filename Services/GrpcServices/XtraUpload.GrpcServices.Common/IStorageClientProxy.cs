using System;
using XtraUpload.Domain;

namespace XtraUpload.GrpcServices.Common
{
    /// <summary>
    /// Singleton instance used to interact with the connected storage clients, retrieve configuration,...
    /// </summary>
    public interface IStorageClientProxy
    {
        /// <summary>
        /// Event raised to make a request to the designated client storage (must be called only in the GrpcServices project)
        /// </summary>
        event EventHandler<UploadOptsRequestedEventArgs> UploadOptsRequested;
        /// <summary>
        /// Sets the upload options for a given server (must be called only in the GrpcServices project)
        /// </summary>
        void SetUploadOptions(UploadOptions response, string callerAddress);
        /// <summary>
        /// Gets the <see cref="UploadOptions"/> for the given server
        /// </summary>
        UploadOptionsResult GetUploadOptions(string serverAddress);
    }
}
