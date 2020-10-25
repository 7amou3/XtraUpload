using XtraUpload.Authentication.Service.Common;
using XtraUpload.Domain;
using XtraUpload.Email.Service.Common;

namespace XtraUpload.Setting.Service.Common
{
    public class ReadAppSettingResult: OperationResult
    {
        public WebAppInfo AppInfo { get; set; }
        public SocialAuthSettings SocialAuthSettings { get; set; }
        public JwtIssuerOptions JwtIssuerOptions { get; set; }
        public HardwareCheckOptions HardwareCheckOptions { get; set; }
        public EmailSettings EmailSettings { get; set; }
    }
}
