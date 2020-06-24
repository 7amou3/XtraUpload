using XtraUpload.Domain;
using XtraUpload.ServerApp.Common;

namespace XtraUpload.Setting.Service.Common
{
    public class ReadAppSettingResult: OperationResult
    {
        public UploadOptions UploadOptions { get; set; }
        public JwtIssuerOptions JwtIssuerOptions { get; set; }
        public HardwareCheckOptions HardwareCheckOptions { get; set; }
        public EmailSettings EmailSettings { get; set; }
    }
}
