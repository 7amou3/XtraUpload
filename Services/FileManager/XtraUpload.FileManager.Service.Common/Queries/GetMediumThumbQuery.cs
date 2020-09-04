using MediatR;

namespace XtraUpload.FileManager.Service.Common
{
    public class GetMediumThumbQuery : IRequest<AvatarUrlResult>
    {
        public GetMediumThumbQuery(string fileid)
        {
            FileId = fileid;
        }
        public string FileId { get; }
    }
}
