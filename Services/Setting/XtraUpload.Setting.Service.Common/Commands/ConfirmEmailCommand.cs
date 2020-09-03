using MediatR;
using XtraUpload.Domain;

namespace XtraUpload.Setting.Service.Common
{
    public class ConfirmEmailCommand : IRequest<OperationResult>
    {
        public ConfirmEmailCommand(string emailToken)
        {
            EmailToken = emailToken;
        }
        public string EmailToken { get; }
    }
}
