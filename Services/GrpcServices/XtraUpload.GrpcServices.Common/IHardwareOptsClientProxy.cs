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
        /// Gets the <see cref="HardwareCheckOptions"/> for the given server
        /// </summary>
        Task<HardwareCheckOptionsResult> ReadHardwareOptions(string serverAddress);
        /// <summary>
        /// Sets the <see cref="HardwareCheckOptions"/> for the given server
        /// </summary>
        Task<OperationResult> WriteHardwareOptions(HardwareCheckOptions hardwareOpts, string serverAddress);
    }
    /// <summary>
    /// Interface used to expose command to the grpc service
    /// </summary>
    public interface IHardwareOptsClientCommand
    {
        /// <summary>
        /// Event raised to read hardware options configuration of the designated client storage
        /// </summary>
        event EventHandler<ReadHardwareOptionsEventArgs> ReadHardwareOptionsRequested;
        /// <summary>
        /// Event raised to write hardware options configuration of the designated client storage
        /// </summary>
        event EventHandler<WriteHardwareOptionsEventArgs> WriteHardwareOptionsRequested;
        /// <summary>
        /// Sets the hardware options for a given server (must be called only in the GrpcServices project)
        /// </summary>
        void SetHardwareOptions(HardwareCheckOptions options, string callerAddress);
    }
}
