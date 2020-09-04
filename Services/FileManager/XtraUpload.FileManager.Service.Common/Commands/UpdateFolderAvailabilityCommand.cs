using MediatR;

namespace XtraUpload.FileManager.Service.Common
{
    /// <summary>
    /// Update online folder availability
    /// </summary>
    public class UpdateFolderAvailabilityCommand : IRequest<FolderAvailabilityResult>
    {
        public UpdateFolderAvailabilityCommand(string folderId, bool isOnline)
        {
            FolderId = folderId;
            IsOnline = isOnline;
        }
        public string FolderId { get; }
        public bool IsOnline { get; set; }
    }
}
