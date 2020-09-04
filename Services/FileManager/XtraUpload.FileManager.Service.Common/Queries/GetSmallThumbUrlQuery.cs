using MediatR;

namespace XtraUpload.FileManager.Service.Common
{
    public class GetSmallThumbUrlQuery : IRequest<AvatarUrlResult>
    {
        public GetSmallThumbUrlQuery(string fileid)
        {
            FileId = fileid;
        }
        public string FileId { get; }
    }
}
