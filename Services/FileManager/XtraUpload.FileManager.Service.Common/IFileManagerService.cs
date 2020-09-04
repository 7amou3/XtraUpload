using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XtraUpload.Domain;
using XtraUpload.WebApp.Common;

namespace XtraUpload.FileManager.Service.Common
{
    public interface IFileManagerService
    {
        Task<AvatarResult> GetUserAvatar();
    }
}
