using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using XtraUpload.Database.Data.Common;
using XtraUpload.Domain;
using XtraUpload.FileManager.Service.Common;

namespace XtraUpload.FileManager.Service
{
    /// <summary>
    /// Recursively get all folders within a folder
    /// </summary>
    public class GetFoldersRecursivelyQueryHandler : IRequestHandler<GetFoldersRecursivelyQuery, IEnumerable<FolderItem>>
    {
        #region Fields
        readonly IUnitOfWork _unitOfWork;
        #endregion

        #region Constructor
        public GetFoldersRecursivelyQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        #endregion

        #region Handler
        public async Task<IEnumerable<FolderItem>> Handle(GetFoldersRecursivelyQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<FolderItem> userFolders = await _unitOfWork.Folders.FindAsync(s => s.UserId == request.Folder.UserId);

            List<FolderItem> childFolders = new List<FolderItem>();

            void _getChildFolders(string id)
            {
                var childs = userFolders.Where(s => s.Parentid == id).ToList();
                childFolders.AddRange(childs);
                // Recursively get child folder
                childs.ForEach(c => _getChildFolders(c.Id));
            }
            _getChildFolders(request.Folder.Id);

            childFolders.Add(request.Folder);
            return childFolders;
        }
        #endregion
    }
}
