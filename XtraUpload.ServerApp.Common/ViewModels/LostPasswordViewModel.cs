using System.ComponentModel.DataAnnotations;

namespace XtraUpload.ServerApp.Common
{
    public class LostPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }
}
