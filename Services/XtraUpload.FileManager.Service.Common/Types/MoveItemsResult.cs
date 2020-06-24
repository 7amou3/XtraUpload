using System.Collections.Generic;
using XtraUpload.Domain;

namespace XtraUpload.FileManager.Service.Common
{
    public class MoveItemsResult: OperationResult
    {
        /// <summary>
        /// Current structure of main folder tree
        /// </summary>
        public IEnumerable<FolderItem> Folders { get; set; }

        /// <summary>
        /// Ids of items (files/folders) successfully moved
        /// </summary>
        public IEnumerable<string> MovedItemsIds { get; set; }
    }
}
