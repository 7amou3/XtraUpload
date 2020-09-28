using System.ComponentModel.DataAnnotations;

namespace XtraUpload.WebApi
{
    public class LostPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }
}
