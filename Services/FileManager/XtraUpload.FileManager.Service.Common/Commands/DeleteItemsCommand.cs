using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using XtraUpload.Domain;

namespace XtraUpload.FileManager.Service.Common
{
    /// <summary>
    /// Delete folders and files
    /// </summary>
    public class DeleteItemsCommand : IRequest<DeleteItemsResult>
    {
        public DeleteItemsCommand(IEnumerable<FolderItem> selectedFolders, IEnumerable<FileItem> selectedFiles)
        {
            SelectedFolders = selectedFolders;
            SelectedFiles = selectedFiles;
        }
        /// <summary>
        /// Files to delete
        /// </summary>
        public IEnumerable<FileItem> SelectedFiles { get; }
        /// <summary>
        /// Folder to delete
        /// </summary>
        public IEnumerable<FolderItem> SelectedFolders { get; }
    }
}
