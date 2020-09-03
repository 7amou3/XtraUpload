using MediatR;
using System.ComponentModel.DataAnnotations;
using XtraUpload.Domain;

namespace XtraUpload.Authentication.Service.Common
{
    public class UpdatePasswordCommand : IRequest<OperationResult>
    {
        [Required]
        [MinLength(6)]
        public string NewPassword { get; set; }

        [Required]
        [MinLength(6)]
        [RegularExpression("^[a-zA-Z0-9]*$")]
        public string RecoveryKey { get; set; }
    }
}
