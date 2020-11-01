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
        /// Gets the <see cref="UploadOptions"/> for the given server
        /// </summary>
        Task<UploadOptionsResult> ReadUploadOptions(string serverAddress);
        /// <summary>
        /// Sets the <see cref="UploadOptions"/> for the given server
        /// </summary>
        Task<OperationResult> WriteUploadOptions(UploadOptions uploadOpts, string serverAddress);
    }
    /// <summary>
    /// Interface used to expose command to the grpc service
    /// </summary>
    public interface IUploadOptsClientCommand
    {
        /// <summary>
        /// Event raised to read upload options of designated client storage
        /// </summary>
        event EventHandler<ReadUploadOptionsEventArgs> ReadUploadOptsRequested;
        /// <summary>
        /// Event raised to write upload options to designated client storage
        /// </summary>
        event EventHandler<WriteUploadOptionsEventArgs> WriteUploadOptsRequested;
        /// <summary>
        /// Sets the upload options for a given server (must be called only in the GrpcServices project)
        /// </summary>
        void SetUploadOptions(UploadOptions response, string callerAddress);
    }
}
