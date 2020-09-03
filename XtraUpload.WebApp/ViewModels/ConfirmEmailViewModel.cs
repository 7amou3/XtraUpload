using System.ComponentModel.DataAnnotations;

namespace XtraUpload.WebApp
{
    public class ConfirmEmailViewModel
    {
        [Required]
        [MinLength(6)]
        [RegularExpression("^[a-zA-Z0-9]*$")]
        public string EmailToken { get; set; }
    }
}
