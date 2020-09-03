using System.ComponentModel.DataAnnotations;

namespace XtraUpload.WebApp
{
    public class UpdatePasswordViewModel
    {
        [Required]
        [MinLength(6)]
        public string OldPassword { get; set; }
        [Required]
        [MinLength(6)]
        public string NewPassword { get; set; }
    }
}
