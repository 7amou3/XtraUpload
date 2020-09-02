using MediatR;
using XtraUpload.Domain;

namespace XtraUpload.Setting.Service.Common
{
    public class RequestConfirmationEmailCommand: IRequest<OperationResult>
    {
        public RequestConfirmationEmailCommand(string userIp)
        {
            UserIp = userIp;
        }
        public string UserIp { get; }
    }
}
