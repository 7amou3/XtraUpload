using System;
using System.Threading.Tasks;
using XtraUpload.Domain;

namespace XtraUpload.GrpcServices.Common
{
    /// <summary>
    /// Singleton instance used to interact with the connected storage clients to retrieve hardware configuration
    /// </summary>
    public interface IHardwareOptsClientProxy
    {
        /// <summary>
        /// Event raised to read hardware options configuration of the designated client storage
        /// </summary>
        event EventHandler<HardwareOptsRequestedEventArgs> HardwareOptionsRequested;
        /// <summary>
        /// Sets the hardware options for a given server (must be called only in the GrpcServices project)
        /// </summary>
        void SetHardwareOptions(HardwareCheckOptions options, string callerAddress);
        /// <summary>
        /// Gets the <see cref="HardwareCheckOptions"/> for the given server
        /// </summary>
        Task<HardwareCheckOptionsResult> GetHardwareOptions(string serverAddress);
    }
}
