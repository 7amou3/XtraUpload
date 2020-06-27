using XtraUpload.WebApp.Common;

namespace XtraUpload.WebApp
{
    internal class ReadAppSettingResultDto
    {
        public WebAppSettings AppSettings { get; set; }
        public SocialAuthSettings SocialAuthSettings { get; set; }
        public UploadOptions UploadOptions { get; set; }
        public JwtIssuerOptionsDto JwtIssuerOptions { get; set; }
        public HardwareCheckOptions HardwareCheckOptions { get; set; }
        public EmailSettings EmailSettings { get; set; }
    }

    internal class JwtIssuerOptionsDto
    {
        public double ValidFor { get; set; }
        public string SecretKey { get; set; }
        public string Audience { get; set; }
        public string Issuer { get; set; }
    }
}
