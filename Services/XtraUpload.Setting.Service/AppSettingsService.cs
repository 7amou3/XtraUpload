using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XtraUpload.Domain;
using XtraUpload.ServerApp.Common;
using XtraUpload.Setting.Service.Common;

namespace XtraUpload.Setting.Service
{
    public class AppSettingsService : IAppSettingsService
    {
        readonly IWritableOptions<JwtIssuerOptions> _JwtOpts;
        readonly IWritableOptions<UploadOptions> _uploadOpts;
        readonly IWritableOptions<EmailSettings> _emailSettings;
        readonly IWritableOptions<HardwareCheckOptions> _hdOpts;
        public AppSettingsService(IWritableOptions<JwtIssuerOptions> jwtOpts, IWritableOptions<UploadOptions> uploadOpts,
            IWritableOptions<EmailSettings> emailSettings, IWritableOptions<HardwareCheckOptions> hdOpts)
        {
            _hdOpts = hdOpts;
            _JwtOpts = jwtOpts;
            _uploadOpts = uploadOpts;
            _emailSettings = emailSettings;
        }

        /// <summary>
        /// Read Appsettings configuraion
        /// </summary>
        /// <returns></returns>
        public ReadAppSettingResult ReadAppSetting()
        {
            return new ReadAppSettingResult()
            {
                EmailSettings = _emailSettings.Value,
                HardwareCheckOptions = _hdOpts.Value,
                JwtIssuerOptions = _JwtOpts.Value,
                UploadOptions = _uploadOpts.Value
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
                    s.Smtp.Username= opts.Smtp.Username;
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
            else
            {
                result.ErrorContent = new ErrorContent("Unknown configuration section", ErrorOrigin.Client);
            }

            return new OperationResult();
        }
    }
}
