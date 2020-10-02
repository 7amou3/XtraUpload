using XtraUpload.Domain;
using XtraUpload.Protos;

namespace XtraUpload.GrpcServices
{
    public static class RequestStatusFactory
    {
        // Domain to proto
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
    }
}
