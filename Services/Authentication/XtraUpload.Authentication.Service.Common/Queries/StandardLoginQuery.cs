using MediatR;
using System.ComponentModel.DataAnnotations;

namespace XtraUpload.Authentication.Service.Common
{
    public class StandardLoginQuery: IRequest<XuIdentityResult>
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
