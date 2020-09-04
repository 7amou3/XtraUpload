using MediatR;

namespace XtraUpload.FileManager.Service.Common
{
    public class CreateFolderCommand : IRequest<CreateFolderResult>
    {
        public CreateFolderCommand(string folderName, string parentFolderId)
        {
            FolderName = folderName;
            ParentFolderId = parentFolderId;
        }
        public string FolderName { get; }
        public string ParentFolderId { get; }
    }
}
