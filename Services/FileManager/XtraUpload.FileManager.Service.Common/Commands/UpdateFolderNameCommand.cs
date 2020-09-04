using MediatR;

namespace XtraUpload.FileManager.Service.Common
{
    /// <summary>
    /// Update folder name
    /// </summary>
    public class UpdateFolderNameCommand : IRequest<RenameFolderResult>
    {
        public UpdateFolderNameCommand(string folderId, string newName)
        {
            FolderId = folderId;
            NewName = newName;
        }
        public string FolderId { get; }
        public string NewName { get; set; }
    }
}
