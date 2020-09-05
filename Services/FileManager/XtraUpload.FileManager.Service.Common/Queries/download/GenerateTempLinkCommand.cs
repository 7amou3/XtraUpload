using MediatR;

namespace XtraUpload.FileManager.Service.Common
{
    /// <summary>
    /// Generate a download templink
    /// </summary>
    public class GenerateTempLinkCommand : IRequest<TempLinkResult>
    {
        public GenerateTempLinkCommand(string fileId)
        {
            FileId = fileId;
        }
        public string FileId { get; set; }
    }
}
