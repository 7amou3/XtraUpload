using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using XtraUpload.Administration.Service.Common;
using XtraUpload.GrpcServices.Common;

namespace XtraUpload.Administration.Service
{
    /// <summary>
    /// Get the upload config of the remote storage server config
    /// </summary>
    public class GetUploadConfigQueryHandler : IRequestHandler<GetUploadConfigConfigQuery, UploadOptionsResult>
    {
        readonly IUploadOptsClientProxy _uploadOptsProxy;
        public GetUploadConfigQueryHandler(IUploadOptsClientProxy uploadOptsProxy)
        {
            _uploadOptsProxy = uploadOptsProxy;
        }
        public async Task<UploadOptionsResult> Handle(GetUploadConfigConfigQuery request, CancellationToken cancellationToken)
        {
            return await _uploadOptsProxy.ReadUploadOptions(request.ServerAddress);
        }
    }
}
