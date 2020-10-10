using MediatR;

namespace XtraUpload.StorageManager.Common
{
    /// <summary>
    /// Get avatar 
    /// </summary>
    public class GetAvatarQuery : IRequest<AvatarUrlResult>
    {
        public GetAvatarQuery(string userid)
        {
            UserId = userid;
        }
        public string UserId { get; }
    }
}
