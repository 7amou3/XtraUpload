using XtraUpload.Domain;
using XtraUpload.Protos;

namespace XtraUpload.GrpcServices
{
    public static class RequestStatusFactory
    {
        // Domain to Proto
        public static gRequestStatus Convert(this OperationResult result)
        {
            if (result == null) return null;

            var status = result.State switch
            {
                OperationState.Success => Protos.RequestStatus.Success,
                OperationState.Warning => Protos.RequestStatus.Warning,
                OperationState.Failed => Protos.RequestStatus.Failed,
                _ => Protos.RequestStatus.Success,
            };

            return new gRequestStatus()
            {
                Message = result.ErrorContent.Message ?? string.Empty,
                Status = status
            };
        }
        // Proto to Domain
        public static OperationResult Convert(this gRequestStatus result)
        {
            if (result == null) return null;

            var opResult = new OperationResult();
            if (result.Status != Protos.RequestStatus.Success)
            {
                opResult.ErrorContent = new ErrorContent(result.Message, ErrorOrigin.None);
            }
            return opResult;
        }
    }
}
