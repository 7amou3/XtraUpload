using MediatR;
using XtraUpload.GrpcServices.Common;

namespace XtraUpload.Administration.Service.Common
{
    /// <summary>
    /// Get the hardware config of the remote storage server
    /// </summary>
    public class GetHardwareConfigQuery : IRequest<HardwareCheckOptionsResult>
    {
        public GetHardwareConfigQuery(string serverAddress)
        {
            ServerAddress = serverAddress;
        }
        public string ServerAddress { get; }
    }
}
