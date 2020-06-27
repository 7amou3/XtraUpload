using XtraUpload.Domain;
using XtraUpload.WebApp.Common;

namespace XtraUpload.Setting.Service.Common
{
    public class ReadAppSettingResult: OperationResult
    {
        public WebAppSettings AppSettings { get; set; }
        public SocialAuthSettings SocialAuthSettings { get; set; }
        public UploadOptions UploadOptions { get; set; }
        public JwtIssuerOptions JwtIssuerOptions { get; set; }
        public HardwareCheckOptions HardwareCheckOptions { get; set; }
        public EmailSettings EmailSettings { get; set; }
    }
}
