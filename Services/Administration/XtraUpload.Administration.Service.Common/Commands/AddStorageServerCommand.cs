using MediatR;
using XtraUpload.Domain;

namespace XtraUpload.Administration.Service.Common
{
    /// <summary>
    /// Add a storage server
    /// </summary>
    public class AddStorageServerCommand : IRequest<StorageServerResult>
    {
        public StorageServer StorageInfo { get; set; }
        public UploadOptions UploadOpts { get; set; }
        public HardwareCheckOptions HardwareOpts { get; set; }
    }
}
