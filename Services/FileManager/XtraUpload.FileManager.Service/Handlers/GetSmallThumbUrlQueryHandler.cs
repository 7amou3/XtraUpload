using MediatR;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XtraUpload.Domain;
using XtraUpload.FileManager.Service.Common;

namespace XtraUpload.FileManager.Service
{
    public class GetSmallThumbUrlQueryHandler : IRequestHandler<GetSmallThumbUrlQuery, AvatarUrlResult>
    {
        #region Fields
        readonly IMediator _mediator;
        readonly UploadOptions _uploadOpt;
        #endregion

        #region Constructor
        public GetSmallThumbUrlQueryHandler(IMediator mediator, IOptionsMonitor<UploadOptions> uploadOpt)
        {
            _mediator = mediator;
            _uploadOpt = uploadOpt.CurrentValue;
        }
        #endregion

        #region Handler
        public async Task<AvatarUrlResult> Handle(GetSmallThumbUrlQuery request, CancellationToken cancellationToken)
        {
            AvatarUrlResult Result = new AvatarUrlResult();

            // Get the file
            GetFileResult fileResult =  await _mediator.Send(new GetFileByIdQuery(request.FileId));

            if (Result.State != OperationState.Success)
            {
                Result = OperationResult.CopyResult<AvatarUrlResult>(fileResult);
                return Result;
            }

            string filePath = Path.Combine(_uploadOpt.UploadPath, fileResult.File.UserId.ToString(), fileResult.File.Id, fileResult.File.Id + ".smallthumb.png");

            if (!File.Exists(filePath))
            {
                Result.ErrorContent = new ErrorContent("File does not exist on the server, it may be moved or deleted.", ErrorOrigin.Client);
                return Result;
            }

            Result.Url = filePath;

            return Result;
        }
        #endregion
    }
}
