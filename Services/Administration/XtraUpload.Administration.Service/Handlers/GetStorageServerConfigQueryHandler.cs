using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using XtraUpload.Administration.Service.Common;
using XtraUpload.GrpcServices.Common;

namespace XtraUpload.Administration.Service.Handlers
{
    /// <summary>
    /// Get the remote storage server config
    /// </summary>
    public class GetStorageServerConfigQueryHandler : IRequestHandler<GetStorageServerConfigQuery, UploadOptionsResult>
    {
        readonly IStorageClientProxy _storageProxy;
        public GetStorageServerConfigQueryHandler(IStorageClientProxy storageProxy)
        {
            _storageProxy = storageProxy;
        }
        public async Task<UploadOptionsResult> Handle(GetStorageServerConfigQuery request, CancellationToken cancellationToken)
        {
            return await _storageProxy.GetUploadOptions(request.ServerAddress);
        }
    }
}
