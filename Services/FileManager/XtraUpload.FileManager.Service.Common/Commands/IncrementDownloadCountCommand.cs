using MediatR;
using XtraUpload.Domain;

namespace XtraUpload.FileManager.Service.Common
{
    /// <summary>
    /// Increment the download count for the given file
    /// </summary>
    public class IncrementDownloadCountCommand: IRequest<OperationResult>
    {
        public IncrementDownloadCountCommand(string fileId, string requesterIp)
        {
            FileId = fileId;
            RequesterIp = requesterIp;
        }
        public string FileId { get; }
        public string RequesterIp { get; }
    }
}
