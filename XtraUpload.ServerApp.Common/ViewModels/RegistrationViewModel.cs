using System.ComponentModel.DataAnnotations;

namespace XtraUpload.WebApp.Common
{
    public class RegistrationViewModel
    {
        [Required]
        [MinLength(4)]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }
    }
}
