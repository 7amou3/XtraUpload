using MediatR;
using XtraUpload.Domain;

namespace XtraUpload.Administration.Service.Common
{
    public class UpdateStorageServerCommand : IRequest<StorageServerResult>
    {
        public string Id { get; set; }
        public StorageServer StorageInfo { get; set; }
        public UploadOptions UploadOpts { get; set; }
        public HardwareCheckOptions HardwareOpts { get; set; }
    }
}
