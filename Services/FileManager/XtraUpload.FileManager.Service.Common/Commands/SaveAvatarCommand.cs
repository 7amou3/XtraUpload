using MediatR;
using XtraUpload.Domain;

namespace XtraUpload.FileManager.Service.Common
{
    /// <summary>
    /// Save the avatar to db
    /// </summary>
    public class SaveAvatarCommand : IRequest<OperationResult>
    {
        public SaveAvatarCommand(string userId, string avatarUrl)
        {
            UserId = userId;
            AvatarUrl = avatarUrl;
        }
        public string UserId { get; }
        public string AvatarUrl { get; }
    }
}
