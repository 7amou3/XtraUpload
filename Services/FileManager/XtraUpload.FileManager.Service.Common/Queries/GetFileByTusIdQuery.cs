using MediatR;

namespace XtraUpload.FileManager.Service.Common
{
    /// <summary>
    /// Get a file by it's tus id
    /// </summary>
    public class GetFileByTusIdQuery : IRequest<GetFileResult>
    {
        public GetFileByTusIdQuery(string tusId)
        {
            TusId = tusId;
        }
        public string TusId { get; }
    }
}
