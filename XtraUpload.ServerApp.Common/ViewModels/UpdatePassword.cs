using System.ComponentModel.DataAnnotations;

namespace XtraUpload.WebApp.Common
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
