using MediatR;
using Microsoft.Extensions.Options;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using XtraUpload.Domain;
using XtraUpload.FileManager.Service.Common;

namespace XtraUpload.FileManager.Service
{
    /// <summary>
    /// Return the full path to a medium or small thumb 
    /// </summary>
    public class GetThumbnailQueryHandler : IRequestHandler<GetThumbnailQuery, AvatarUrlResult>
    {
        readonly IMediator _mediator;
        readonly UploadOptions _uploadOpt;
        
        public GetThumbnailQueryHandler(IMediator mediator, IOptionsMonitor<UploadOptions> uploadOpt)
        {
            _mediator = mediator;
            _uploadOpt = uploadOpt.CurrentValue;
        }
        
        public async Task<AvatarUrlResult> Handle(GetThumbnailQuery request, CancellationToken cancellationToken)
        {
            AvatarUrlResult Result = new AvatarUrlResult();

            // Get the file
            GetFileResult fileResult = await _mediator.Send(new GetFileServerInfoQuery(request.FileId));

            if (Result.State != OperationState.Success)
            {
                Result = OperationResult.CopyResult<AvatarUrlResult>(fileResult);
                return Result;
            }
            string filePath = null;
            switch (request.ThumbnailSize)
            {
                case ThumbnailSize.Small:
                    filePath = Path.Combine(_uploadOpt.UploadPath, fileResult.File.UserId.ToString(), fileResult.File.Id, fileResult.File.Id + ".smallthumb.png");
                    break;
                case ThumbnailSize.Medium:
                    filePath = Path.Combine(_uploadOpt.UploadPath, fileResult.File.UserId.ToString(), fileResult.File.Id, fileResult.File.Id + ".mediumthumb.png");
                    break;
                default:
                    break;
            }
            // todo: handle the case where the image has no medium thumb because it's small
            if (!File.Exists(filePath))
            {
                Result.ErrorContent = new ErrorContent("File does not exist on the server, it may be moved or deleted.", ErrorOrigin.Client);
                return Result;
            }
            
            Result.Url = filePath;

            return Result;
        }
    }
}
