using MediatR;
using XtraUpload.Domain;

namespace XtraUpload.FileManager.Service.Common
{
    /// <summary>
    /// Save the avatar to db
    /// </summary>
    public class SaveAvatarCommand : IRequest<OperationResult>
    {
        public SaveAvatarCommand(string avatarUrl)
        {
            AvatarUrl = avatarUrl;
        }
        public string AvatarUrl { get; }
    }
}
