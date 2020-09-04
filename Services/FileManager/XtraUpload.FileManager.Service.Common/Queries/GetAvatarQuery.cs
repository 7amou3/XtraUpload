using MediatR;

namespace XtraUpload.FileManager.Service.Common
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
