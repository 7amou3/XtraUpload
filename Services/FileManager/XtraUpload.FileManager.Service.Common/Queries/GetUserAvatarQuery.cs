using MediatR;

namespace XtraUpload.FileManager.Service.Common
{
    /// <summary>
    /// Get user's avatar url
    /// </summary>
    public class GetUserAvatarQuery : IRequest<AvatarUrlResult>
    {
    }
}
