using MediatR;
using System.Threading;
using System.Threading.Tasks;
using XtraUpload.Administration.Service.Common;
using XtraUpload.Database.Data.Common;

namespace XtraUpload.Administration.Service
{
    /// <summary>
    /// Get available file extensions
    /// </summary>
    public class GetFileExtensionsQueryHandler : IRequestHandler<GetFileExtensionsQuery, FileExtensionsResult>
    {
        readonly IUnitOfWork _unitOfWork;
        public GetFileExtensionsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<FileExtensionsResult> Handle(GetFileExtensionsQuery request, CancellationToken cancellationToken)
        {
            var Result = new FileExtensionsResult()
            {
                FileExtensions = await _unitOfWork.FileExtensions.GetAll()
            };
            return Result;
        }
    }
}
