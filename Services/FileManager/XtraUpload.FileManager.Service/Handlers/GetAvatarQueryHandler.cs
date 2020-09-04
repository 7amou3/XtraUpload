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
    /// <summary>
    /// Get avatar 
    /// </summary>
    public class GetAvatarQueryHandler : IRequestHandler<GetAvatarQuery, AvatarUrlResult>
    {
        #region Fields
        UploadOptions _uploadOpt;
        #endregion

        #region Constructor
        public GetAvatarQueryHandler(IOptionsMonitor<UploadOptions> uploadOpt)
        {
            _uploadOpt = uploadOpt.CurrentValue;
        }
        #endregion

        #region Handler
        public async Task<AvatarUrlResult> Handle(GetAvatarQuery request, CancellationToken cancellationToken)
        {
            AvatarUrlResult Result = new AvatarUrlResult();
            // Check guid valid
            if (!Guid.TryParse(request.UserId, out _))
            {
                Result.ErrorContent = new ErrorContent("Invalid user id.", ErrorOrigin.Client);
                return Result;
            }
            // Get file path
            string filePath = Path.Combine(_uploadOpt.UploadPath, request.UserId, "avatar", "avatar.png");
            // Check file exist on disk
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
