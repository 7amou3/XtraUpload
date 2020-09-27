using System.ComponentModel.DataAnnotations;

namespace XtraUpload.WebApi
{
    public class AddExtensionViewModel
    {
        [Required]
        [MaxLength(10)]
        [RegularExpression(@"^\.[a-zA-Z0-9]*$")]
        public string Name { get; set; }
    }
}
