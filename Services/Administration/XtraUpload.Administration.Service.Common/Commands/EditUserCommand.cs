using MediatR;
using System.ComponentModel.DataAnnotations;

namespace XtraUpload.Administration.Service.Common
{
    /// <summary>
    /// Edit a user
    /// </summary>
    public class EditUserCommand : IRequest<EditUserResult>
    {
        [Required]
        public string Id { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public string NewPassword { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool SuspendAccount { get; set; }
        public string RoleId { get; set; }
    }
}
