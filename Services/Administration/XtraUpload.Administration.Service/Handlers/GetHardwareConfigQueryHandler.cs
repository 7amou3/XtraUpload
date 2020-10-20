using MediatR;
using System.Threading;
using System.Threading.Tasks;
using XtraUpload.Administration.Service.Common;
using XtraUpload.GrpcServices.Common;

namespace XtraUpload.Administration.Service
{
    /// <summary>
    /// Get the hardware config of the remote storage server
    /// </summary>
    public class GetHardwareConfigQueryHandler : IRequestHandler<GetHardwareConfigQuery, HardwareCheckOptionsResult>
    {
        readonly IHardwareOptsClientProxy _hardwareOptsProxy;
        public GetHardwareConfigQueryHandler(IHardwareOptsClientProxy hardwareOptsProxy)
        {
            _hardwareOptsProxy = hardwareOptsProxy;
        }
        public async Task<HardwareCheckOptionsResult> Handle(GetHardwareConfigQuery request, CancellationToken cancellationToken)
        {
            return await _hardwareOptsProxy.ReadHardwareOptions(request.ServerAddress);
        }
    }
}
