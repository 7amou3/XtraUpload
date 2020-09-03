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

        /// <summary>
        /// Updates appsettings section
        /// </summary>
        public async Task<OperationResult> UpdateSection<T>(T model) where T : class
        {
            OperationResult result = new OperationResult();

            if (model is UploadOptions)
            {
                await _uploadOpts.Update(s =>
                {
                    var opts = model as UploadOptions;
                    s.ChunkSize = opts.ChunkSize;
                    s.Expiration = opts.Expiration;
                    s.UploadPath = opts.UploadPath;
                });
            }
            else if (model is HardwareCheckOptions)
            {
                await _hdOpts.Update(s =>
                {
                    var opts = model as HardwareCheckOptions;
                    s.MemoryThreshold = opts.MemoryThreshold;
                    s.StorageThreshold = opts.StorageThreshold;
                });
            }
            else if (model is EmailSettings)
            {
                await _emailSettings.Update(s =>
                {
                    var opts = model as EmailSettings;
                    s.Smtp.Server = opts.Smtp.Server;
                    s.Smtp.Port = opts.Smtp.Port;
                    s.Smtp.Username = opts.Smtp.Username;
                    s.Smtp.Password = opts.Smtp.Password;
                    s.Sender.Name = opts.Sender.Name;
                    s.Sender.Support = opts.Sender.Support;
                    s.Sender.Admin = opts.Sender.Admin;
                });
            }
            else if (model is JwtIssuerOptions)
            {
                await _JwtOpts.Update(s =>
                {
                    var opts = model as JwtIssuerOptions;
                    s.Audience = opts.Audience;
                    s.SecretKey = opts.SecretKey;
                    s.Issuer = opts.Issuer;
                    s.ValidFor = opts.ValidFor;
                });
            }
            else if (model is WebAppSettings)
            {
                await _appSettings.Update(s =>
                {
                    var settings = model as WebAppSettings;
                    s.Title = settings.Title;
                    s.Description = settings.Description;
                    s.Keywords = settings.Keywords;
                });
            }
            else if (model is SocialAuthSettings)
            {
                await _socialSettings.Update(s =>
                {
                    var settings = model as SocialAuthSettings;
                    s.FacebookAuth.AppId = settings.FacebookAuth.AppId;
                    s.GoogleAuth.ClientId = settings.GoogleAuth.ClientId;
                });
            }
            else
            {
                result.ErrorContent = new ErrorContent("Unknown configuration section", ErrorOrigin.Client);
            }

            return new OperationResult();
        }
    }
}
