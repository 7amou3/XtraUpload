using MediatR;
using System.Threading;
using System.Threading.Tasks;
using XtraUpload.Administration.Service.Common;
using XtraUpload.Database.Data.Common;

namespace XtraUpload.Administration.Service
{
    public class GetStorageServersQueryHandler : IRequestHandler<GetStorageServersQuery, StorageServersResult>
    {
        readonly IUnitOfWork _unitOfWork;

        public GetStorageServersQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<StorageServersResult> Handle(GetStorageServersQuery request, CancellationToken cancellationToken)
        {
            return new StorageServersResult
            {
                Servers = await _unitOfWork.StorageServer.GetAll()
            };
        }
    }
}
