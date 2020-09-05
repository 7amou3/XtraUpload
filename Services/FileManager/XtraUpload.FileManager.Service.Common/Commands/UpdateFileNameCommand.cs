using MediatR;

namespace XtraUpload.FileManager.Service.Common
{
    /// <summary>
    /// Update the file name
    /// </summary>
    public class UpdateFileNameCommand : IRequest<RenameFileResult>
    {
        public UpdateFileNameCommand(string fileId, string newName)
        {
            FileId = fileId;
            NewName = newName;
        }
        public string FileId { get; }
        public string NewName { get; }
    }
}
