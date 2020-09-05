using MediatR;

namespace XtraUpload.FileManager.Service.Common
{
    /// <summary>
    /// Request to download a file
    /// </summary>
    public class RequestDownloadQuery : IRequest<RequestDownloadResult>
    {
        public RequestDownloadQuery(string fileid)
        {
            FileId = fileid;
        }
        public string FileId { get; }
    }
}
