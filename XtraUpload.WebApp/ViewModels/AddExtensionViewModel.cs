using System.ComponentModel.DataAnnotations;

namespace XtraUpload.WebApp
{
    public class AddExtensionViewModel
    {
        [Required]
        [MaxLength(10)]
        [RegularExpression(@"^\.[a-zA-Z0-9]*$")]
        public string Name { get; set; }
    }
}
