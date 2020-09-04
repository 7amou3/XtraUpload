using MediatR;
using System.Threading;
using System.Threading.Tasks;
using XtraUpload.Database.Data.Common;
using XtraUpload.Domain;
using XtraUpload.FileManager.Service.Common;

namespace XtraUpload.FileManager.Service
{
    /// <summary>
    /// Get a file by it's id
    /// </summary>
    public class GetFileByIdQueryHandler : IRequestHandler<GetFileByIdQuery, GetFileResult>
    {
        #region Fields
        readonly IUnitOfWork _unitOfWork;
        #endregion

        #region Constructor
        public GetFileByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        #endregion

        #region Handler
        public async Task<GetFileResult> Handle(GetFileByIdQuery request, CancellationToken cancellationToken)
        {
            GetFileResult Result = new GetFileResult()
            {
                File = await _unitOfWork.Files.FirstOrDefaultAsync(s => s.Id == request.FileId)
            };

            // Check if file exist
            if (Result.File == null)
            {
                Result.ErrorContent = new ErrorContent("No file with the provided id was found", ErrorOrigin.Client);
            }

            return Result;
        }
        #endregion
    }
}
