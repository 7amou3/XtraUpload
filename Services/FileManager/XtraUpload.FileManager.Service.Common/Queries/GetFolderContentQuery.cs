using MediatR;

namespace XtraUpload.FileManager.Service.Common
{
    public class GetFolderContentQuery : IRequest<GetFolderContentResult>
    {
        public GetFolderContentQuery(string folderid)
        {
            FolderId = folderid;
        }
        public string FolderId { get; }
    }
}
