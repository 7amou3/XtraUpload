using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using XtraUpload.Domain;

namespace XtraUpload.WebApp
{
    public class MoveItemsViewModel
    {
        /// <summary>
        /// Destination folder
        /// </summary>
        [Required]
        [RegularExpression("^[a-zA-Z0-9]*$")]
        public string DestFolderId { get; set; }
        /// <summary>
        /// Files to move
        /// </summary>
        [Required]
        public IEnumerable<FileItem> SelectedFiles { get; set; }
        /// <summary>
        /// Files to move
        /// </summary>
        [Required]
        public IEnumerable<FolderItem> SelectedFolders { get; set; }
    }
}
