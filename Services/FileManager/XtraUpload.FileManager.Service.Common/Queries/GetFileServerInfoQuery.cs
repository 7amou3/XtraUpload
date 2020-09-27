using MediatR;

namespace XtraUpload.FileManager.Service.Common
{
    /// <summary>
    /// Get a file by it's id
    /// </summary>
    public class GetFileServerInfoQuery : IRequest<GetFileResult>
    {
        public GetFileServerInfoQuery(string fileid)
        {
            FileId = fileid;
        }
        public string FileId { get; }
    }
}
