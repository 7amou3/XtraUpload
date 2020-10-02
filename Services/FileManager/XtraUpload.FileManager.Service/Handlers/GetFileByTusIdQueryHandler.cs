using MediatR;
using Microsoft.AspNetCore.Http;
using System.Linq;
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
            GetFileResult Result = new GetFileResult();
            string userId = _caller.GetUserId();

            var filesResult = await _unitOfWork.Files.GetFilesServerInfo(s => s.UserId == userId && s.TusId == request.TusId);
            if (filesResult.Any())
            {
                Result.File = filesResult.ElementAt(0);
            }
            // Check if file exist
            else
            {
                Result.ErrorContent = new ErrorContent("No file with the provided id was found", ErrorOrigin.Client);
            }

            return Result;
        }
        #endregion
    }
}
