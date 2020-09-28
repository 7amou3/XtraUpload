using MediatR;

namespace XtraUpload.FileManager.Service.Common
{
    public class GetDownloadByIdQuery : IRequest<DownloadedFileResult>
    {
        public GetDownloadByIdQuery(string downloadId, string requesterAddress)
        {
            DownloadId = downloadId;
            RequesterAddress = requesterAddress;
        }
        public string DownloadId { get; }
        public string RequesterAddress { get; }
    }
}
