using XtraUpload.Domain;

namespace XtraUpload.GrpcServices.Common
{
    public class UploadOptionsResult : OperationResult
    {
        public UploadOptions UploadOpts { get; set; }
    }
}
