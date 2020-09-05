using MediatR;
using System.Collections.Generic;
using XtraUpload.Domain;

namespace XtraUpload.FileManager.Service.Common
{
    public class GetFoldersRecursivelyQuery : IRequest<IEnumerable<FolderItem>>
    {
        public GetFoldersRecursivelyQuery(FolderItem folder)
        {
            Folder = folder;
        }
        public FolderItem Folder { get; }
    }
}
