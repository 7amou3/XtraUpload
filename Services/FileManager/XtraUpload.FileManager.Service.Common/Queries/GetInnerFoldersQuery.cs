using MediatR;

namespace XtraUpload.FileManager.Service.Common
{
    public class GetInnerFoldersQuery : IRequest<GetFoldersResult>
    {
        public GetInnerFoldersQuery(string parentId)
        {
            ParentId = parentId;
        }
        public string ParentId { get; }
    }
}
