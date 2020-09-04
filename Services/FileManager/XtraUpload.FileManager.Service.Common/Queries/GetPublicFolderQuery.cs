using MediatR;

namespace XtraUpload.FileManager.Service.Common
{
    /// <summary>
    /// Get a public folder content
    /// </summary>
    public class GetPublicFolderQuery : IRequest<GetFolderContentResult>
    {
        public GetPublicFolderQuery(string mainFolderId, string childFolderId)
        {
            MainFolderId = mainFolderId;
            ChildFolderId = childFolderId;
        }
        public string MainFolderId { get; }
        public string ChildFolderId { get; }
    }
}
