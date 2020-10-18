using System;
using System.Threading.Tasks;
using XtraUpload.Domain;

namespace XtraUpload.GrpcServices.Common
{
    /// <summary>
    /// Singleton instance used to interact with the connected storage clients to retrieve upload configuration
    /// </summary>
    public interface IUploadOptsClientProxy
    {
        /// <summary>
        /// Event raised to read upload options of designated client storage
        /// </summary>
        event EventHandler<UploadOptsRequestedEventArgs> UploadOptsRequested;
        /// <summary>
        /// Sets the upload options for a given server (must be called only in the GrpcServices project)
        /// </summary>
        void SetUploadOptions(UploadOptions response, string callerAddress);
        /// <summary>
        /// Gets the <see cref="UploadOptions"/> for the given server
        /// </summary>
        Task<UploadOptionsResult> GetUploadOptions(string serverAddress);
    }
}
