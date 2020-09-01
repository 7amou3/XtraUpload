using System.ComponentModel.DataAnnotations;

namespace XtraUpload.WebApp
{
    public class CreateUserViewModel
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
