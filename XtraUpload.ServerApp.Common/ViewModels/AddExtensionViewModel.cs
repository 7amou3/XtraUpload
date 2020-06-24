using System.ComponentModel.DataAnnotations;

namespace XtraUpload.ServerApp.Common
{
    public class AddExtensionViewModel
    {
        [Required]
        [MaxLength(10)]
        [RegularExpression(@"^\.[a-zA-Z0-9]*$")]
        public string Name { get; set; }
    }
}
