using System.ComponentModel.DataAnnotations;

namespace XtraUpload.WebApp
{
    public class CreateFolderViewModel
    {
        [Required]
        public string FolderName { get; set; }
        public FolderViewModel ParentFolder { get; set; }
    }
}
