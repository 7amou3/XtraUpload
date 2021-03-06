﻿using MediatR;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XtraUpload.Domain;
using XtraUpload.StorageManager.Common;

namespace XtraUpload.StorageManager.Service
{
    /// <summary>
    /// Get avatar 
    /// </summary>
    public class GetAvatarQueryHandler : IRequestHandler<GetAvatarQuery, AvatarUrlResult>
    {
        readonly UploadOptions _uploadOpt;
        
        public GetAvatarQueryHandler(IOptionsMonitor<UploadOptions> uploadOpt)
        {
            _uploadOpt = uploadOpt.CurrentValue;
        }
        
        public Task<AvatarUrlResult> Handle(GetAvatarQuery request, CancellationToken cancellationToken)
        {
            AvatarUrlResult Result = new AvatarUrlResult();
            // Check guid valid
            if (!Guid.TryParse(request.UserId, out _))
            {
                Result.ErrorContent = new ErrorContent("Invalid user id.", ErrorOrigin.Client);
                return Task.FromResult(Result);
            }
            // Get file path
            string filePath = Path.Combine(_uploadOpt.UploadPath, request.UserId, "avatar", "avatar.png");
            // Check file exist on disk
            if (!File.Exists(filePath))
            {
                Result.ErrorContent = new ErrorContent("File does not exist on the server, it may be moved or deleted.", ErrorOrigin.Client);
                return Task.FromResult(Result);
            }

            Result.Url = filePath;

            return Task.FromResult(Result);
        }
    }
}
