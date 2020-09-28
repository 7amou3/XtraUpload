using System.ComponentModel.DataAnnotations;

namespace XtraUpload.WebApi
{
    public class CreateFolderViewModel
    {
        [Required]
        public string FolderName { get; set; }
        public FolderViewModel ParentFolder { get; set; }
    }
}
