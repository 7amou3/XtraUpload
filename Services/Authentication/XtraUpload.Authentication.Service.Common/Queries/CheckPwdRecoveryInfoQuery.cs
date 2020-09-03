using MediatR;
using XtraUpload.Domain;

namespace XtraUpload.Authentication.Service.Common
{
    public class CheckPwdRecoveryInfoQuery: IRequest<OperationResult>
    {
        public CheckPwdRecoveryInfoQuery(string recoeryId)
        {
            RecoeryId = recoeryId;
        }
        public string RecoeryId { get; }
    }
}
