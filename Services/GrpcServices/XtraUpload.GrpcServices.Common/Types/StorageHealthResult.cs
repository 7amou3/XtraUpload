using XtraUpload.Domain;

namespace XtraUpload.GrpcServices.Common
{
    public class StorageHealthResult : OperationResult
    {
        public string ServerAddress { get; set; }
        public StorageSpaceInfo StorageInfo { get; set; }
        // To do: Memory and Cpu info
    }
}
