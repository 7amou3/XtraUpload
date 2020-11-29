using Askmethat.Aspnet.JsonLocalizer.Localizer;
using MediatR;
using System;
using System.Linq;
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
    public class GetFileServerInfoQueryHandler : IRequestHandler<GetFileServerInfoQuery, GetFileResult>
    {
        #region Fields
        readonly IUnitOfWork _unitOfWork;
        readonly IJsonStringLocalizer _localizer;
        #endregion

        #region Constructor
        public GetFileServerInfoQueryHandler(IUnitOfWork unitOfWork, IJsonStringLocalizer localizer)
        {
            _unitOfWork = unitOfWork;
            _localizer = localizer;
        }
        #endregion

        #region Handler
        public async Task<GetFileResult> Handle(GetFileServerInfoQuery request, CancellationToken cancellationToken)
        {
            GetFileResult Result = new GetFileResult();

            var files = await _unitOfWork.Files.GetFilesServerInfo(s => s.Id == request.FileId);
            if (files.Any())
            {
                Result.File = files.ElementAt(0);
            }
            else
            {
                Result.ErrorContent = new ErrorContent(_localizer["No file with the provided id was found"], ErrorOrigin.Client);
            }

            return Result;
        }
        #endregion
    }
}
