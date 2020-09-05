using MediatR;
using XtraUpload.Domain;

namespace XtraUpload.FileManager.Service.Common
{
    public class GetThumbnailQuery : IRequest<AvatarUrlResult>
    {
        public GetThumbnailQuery(ThumbnailSize thumbSize, string fileid)
        {
            ThumbnailSize = thumbSize;
            FileId = fileid;
        }
        public ThumbnailSize ThumbnailSize { get; }
        public string FileId { get; }
    }
}
