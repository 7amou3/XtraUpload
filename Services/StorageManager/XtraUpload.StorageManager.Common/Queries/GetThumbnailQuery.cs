using MediatR;
using XtraUpload.Domain;

namespace XtraUpload.StorageManager.Common
{
    /// <summary>
    /// Return the full path to a medium or small thumb 
    /// </summary>
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
