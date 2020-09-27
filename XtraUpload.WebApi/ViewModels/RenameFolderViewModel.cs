using System.ComponentModel.DataAnnotations;

namespace XtraUpload.WebApi
{
    public class RenameFolderViewModel
    {
        [Required]
        [MinLength(8)]
        [RegularExpression("^[a-zA-Z0-9]*$")]
        public string FileId { get; set; }

        [Required]
        [MinLength(4)]
        public string NewName { get; set; }
    }
}
