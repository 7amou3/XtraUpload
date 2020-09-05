using MediatR;
using System.Collections.Generic;
using XtraUpload.Domain;

namespace XtraUpload.FileManager.Service.Common
{
    /// <summary>
    /// Move the selected items (files and/or folders) to the specified folder
    /// </summary>
    public class MoveItemsCommand : IRequest<MoveItemsResult>
    {
        public MoveItemsCommand(string destFolderId, IEnumerable<FileItem> selectedFiles, IEnumerable<FolderItem> selectedFolders)
        {
            DestFolderId = destFolderId;
            SelectedFiles = selectedFiles;
            SelectedFolders = selectedFolders;
        }
        /// <summary>
        /// Destination folder
        /// </summary>
        public string DestFolderId { get; }
        /// <summary>
        /// Files to move
        /// </summary>
        public IEnumerable<FileItem> SelectedFiles { get; }
        /// <summary>
        /// Files to move
        /// </summary>
        public IEnumerable<FolderItem> SelectedFolders { get; }
    }
}
