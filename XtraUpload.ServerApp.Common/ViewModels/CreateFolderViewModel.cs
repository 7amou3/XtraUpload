using System.ComponentModel.DataAnnotations;

namespace XtraUpload.ServerApp.Common
{
    public class CreateFolderViewModel
    {
        [Required]
        public string FolderName { get; set; }
        public FolderViewModel ParentFolder { get; set; }
    }
}
