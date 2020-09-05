using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using XtraUpload.Database.Data.Common;
using XtraUpload.Domain;
using XtraUpload.Domain.Infra;
using XtraUpload.FileManager.Service.Common;

namespace XtraUpload.FileManager.Service.Handlers
{
    /// <summary>
    /// Get a file by it's tus id
    /// </summary>
    public class GetFileByTusIdQueryHandler : IRequestHandler<GetFileByTusIdQuery, GetFileResult>
    {
        #region Fields
        readonly IUnitOfWork _unitOfWork;
        readonly ClaimsPrincipal _caller;
        #endregion

        #region Constructor
        public GetFileByTusIdQueryHandler(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _caller = httpContextAccessor.HttpContext.User;
        }
        #endregion

        #region Handler
        public async Task<GetFileResult> Handle(GetFileByTusIdQuery request, CancellationToken cancellationToken)
        {
            string userId = _caller.GetUserId();

            GetFileResult Result = new GetFileResult()
            {
                File = await _unitOfWork.Files.FirstOrDefaultAsync(s => s.UserId == userId && s.TusId == request.TusId)
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
