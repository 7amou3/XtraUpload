using MediatR;

namespace XtraUpload.FileManager.Service.Common
{
    /// <summary>
    /// Get a file by it's id
    /// </summary>
    public class GetFileByIdQuery : IRequest<GetFileResult>
    {
        public GetFileByIdQuery(string fileid)
        {
            FileId = fileid;
        }
        public string FileId { get; }
    }
}
