using System.ComponentModel.DataAnnotations;

namespace XtraUpload.WebApp.Common
{
    public class CredentialsViewModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
