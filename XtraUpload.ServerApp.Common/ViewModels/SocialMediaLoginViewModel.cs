using System.ComponentModel.DataAnnotations;

namespace XtraUpload.ServerApp.Common
{
    public class SocialMediaLoginViewModel
    {
        [Required]
        public string Id { get; set; }
        [Required]
        public string Provider { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string PhotoUrl { get; set; }
        public string FirstName  {get; set; }
        public string LastName { get; set; }
        [Required]
        public string AuthToken { get; set; }
        public string IdToken { get; set; }
    }
}
