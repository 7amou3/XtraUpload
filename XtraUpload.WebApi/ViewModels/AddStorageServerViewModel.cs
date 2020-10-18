using XtraUpload.Domain;

namespace XtraUpload.WebApi
{
    public class AddStorageServerViewModel
    {
        public StorageServer StorageInfo { get; set; }
        public UploadOptions UploadOpts { get; set; }
        public HardwareCheckOptions HardwareOpts { get; set; }
    }
}
