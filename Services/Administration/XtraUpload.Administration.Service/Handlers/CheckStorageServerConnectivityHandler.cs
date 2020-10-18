using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XtraUpload.Administration.Service.Common;
using XtraUpload.Domain;
using XtraUpload.GrpcServices.Common;

namespace XtraUpload.Administration.Service
{
    public class CheckStorageServerConnectivityHandler : IRequestHandler<CheckStorageServerConnectivityQuery, OperationResult>
    {
        readonly ICheckClientProxy _checkClientProxy;
        public CheckStorageServerConnectivityHandler(ICheckClientProxy checkClientProxy)
        {
            _checkClientProxy = checkClientProxy;
        }
        public async Task<OperationResult> Handle(CheckStorageServerConnectivityQuery request, CancellationToken cancellationToken)
        {
            var res = await _checkClientProxy.CheckServerStorageConnectivity(request.ServerAddress);
            return OperationResult.CopyResult<OperationResult>(res);
        }
    }
}
