using MediatR;

namespace XtraUpload.FileManager.Service.Common
{
    /// <summary>
    /// Updates the online availability state of the given file
    /// </summary>
    public class UpdateFileAvailabilityCommand : IRequest<FileAvailabilityResult>
    {
        public UpdateFileAvailabilityCommand(string fileId, bool isOnline)
        {
            FileId = fileId;
            IsOnline = isOnline;
        }
        public string FileId { get; }
        public bool IsOnline { get; }
    }
}
