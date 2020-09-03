using System.Threading.Tasks;
using XtraUpload.Domain;
using XtraUpload.WebApp.Common;
using XtraUpload.Setting.Service.Common;
using XtraUpload.Authentication.Service.Common;
using Microsoft.Extensions.Options;
using XtraUpload.FileManager.Service.Common;
using XtraUpload.Email.Service.Common;

namespace XtraUpload.Setting.Service
{
    public class AppSettingsService : IAppSettingsService
    {
        readonly IWritableOptions<JwtIssuerOptions> _JwtOpts;
        readonly IWritableOptions<UploadOptions> _uploadOpts;
        readonly IWritableOptions<EmailSettings> _emailSettings;
        readonly IWritableOptions<HardwareCheckOptions> _hdOpts;
        readonly IWritableOptions<WebAppSettings> _appSettings;
        readonly IWritableOptions<SocialAuthSettings> _socialSettings;
        public AppSettingsService(IWritableOptions<JwtIssuerOptions> jwtOpts, IWritableOptions<UploadOptions> uploadOpts,
            IWritableOptions<EmailSettings> emailSettings, IWritableOptions<HardwareCheckOptions> hdOpts,
            IWritableOptions<WebAppSettings> appSettings, IWritableOptions<SocialAuthSettings> socialSettings)
        {
            _hdOpts = hdOpts;
            _JwtOpts = jwtOpts;
            _uploadOpts = uploadOpts;
            _appSettings = appSettings;
            _emailSettings = emailSettings;
            _socialSettings = socialSettings;
        }

        /// <summary>
        /// Read Appsettings configuraion
        /// </summary>
        /// <returns></returns>
        public ReadAppSettingResult ReadAppSetting()
        {
            return new ReadAppSettingResult()
            {
                AppSettings = _appSettings.Value,
                EmailSettings = _emailSettings.Value,
                HardwareCheckOptions = _hdOpts.Value,
                JwtIssuerOptions = _JwtOpts.Value,
                UploadOptions = _uploadOpts.Value,
                SocialAuthSettings = _socialSettings.Value
            };
        }

    }
}
