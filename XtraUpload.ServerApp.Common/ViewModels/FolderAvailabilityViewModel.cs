using System.ComponentModel.DataAnnotations;

namespace XtraUpload.ServerApp.Common
{
    public class FolderAvailabilityViewModel
    {
        [Required]
        [RegularExpression("^[a-zA-Z0-9]*$")]
        public string Folderid { get; set; }
        public bool Available { get; set; }
    }
}
