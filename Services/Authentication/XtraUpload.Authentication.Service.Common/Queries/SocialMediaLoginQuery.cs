using MediatR;
using System.ComponentModel.DataAnnotations;

namespace XtraUpload.Authentication.Service.Common
{
    public class SocialMediaLoginQuery: IRequest<XuIdentityResult>
    {
        [Required]
        public string Id { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Provider { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string PhotoUrl { get; set; }
        public string FirstName  {get; set; }
        public string LastName { get; set; }
        [Required]
        public string AuthToken { get; set; }
        public string IdToken { get; set; }
        public string Language { get; set; }
    }
}
