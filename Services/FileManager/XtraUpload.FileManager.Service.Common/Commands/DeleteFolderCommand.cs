using MediatR;

namespace XtraUpload.FileManager.Service.Common
{
    /// <summary>
    /// Delete a folder and all it's content
    /// </summary>
    public class DeleteFolderCommand : IRequest<DeleteFolderResult>
    {
        public DeleteFolderCommand(string folderid)
        {
            FolderId = folderid;
        }
        public string FolderId { get; }
    }
}
