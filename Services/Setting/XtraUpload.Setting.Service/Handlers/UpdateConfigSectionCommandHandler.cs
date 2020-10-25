using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XtraUpload.Authentication.Service.Common;
using XtraUpload.Domain;
using XtraUpload.Email.Service.Common;
using XtraUpload.Setting.Service.Common;

namespace XtraUpload.Setting.Service
{
    /// <summary>
    /// Updates appsettings section
    /// </summary>
    public class UpdateConfigSectionCommandHandler : IRequestHandler<UpdateConfigSectionCommand, OperationResult>
    {
        readonly IWritableOptions<JwtIssuerOptions> _JwtOpts;
        readonly IWritableOptions<EmailSettings> _emailSettings;
        readonly IWritableOptions<HardwareCheckOptions> _hdOpts;
        readonly IWritableOptions<WebAppInfo> _appInfo;
        readonly IWritableOptions<SocialAuthSettings> _socialSettings;
       
        public UpdateConfigSectionCommandHandler(
            IWritableOptions<JwtIssuerOptions> jwtOpts,
            IWritableOptions<EmailSettings> emailSettings,
            IWritableOptions<HardwareCheckOptions> hdOpts,
            IWritableOptions<WebAppInfo> appInfo, 
            IWritableOptions<SocialAuthSettings> socialSettings)
        {
            _hdOpts = hdOpts;
            _JwtOpts = jwtOpts;
            _appInfo = appInfo;
            _emailSettings = emailSettings;
            _socialSettings = socialSettings;
        }

        public async Task<OperationResult> Handle(UpdateConfigSectionCommand request, CancellationToken cancellationToken)
        {
            OperationResult result = new OperationResult();

            if (request.ConfigSection is Domain.UploadOptions)
            {
                //await _uploadOpts.Update(s =>
                //{
                //    var opts = request.ConfigSection as UploadOptions;
                //    s.ChunkSize = opts.ChunkSize;
                //    s.Expiration = opts.Expiration;
                //    s.UploadPath = opts.UploadPath;
                //});
            }
            else if (request.ConfigSection is HardwareCheckOptions)
            {
                await _hdOpts.Update(s =>
                {
                    var opts = request.ConfigSection as HardwareCheckOptions;
                    s.MemoryThreshold = opts.MemoryThreshold;
                    s.StorageThreshold = opts.StorageThreshold;
                });
            }
            else if (request.ConfigSection is EmailSettings)
            {
                await _emailSettings.Update(s =>
                {
                    var opts = request.ConfigSection as EmailSettings;
                    s.Smtp.Server = opts.Smtp.Server;
                    s.Smtp.Port = opts.Smtp.Port;
                    s.Smtp.Username = opts.Smtp.Username;
                    s.Smtp.Password = opts.Smtp.Password;
                    s.Sender.Name = opts.Sender.Name;
                    s.Sender.Support = opts.Sender.Support;
                    s.Sender.Admin = opts.Sender.Admin;
                });
            }
            else if (request.ConfigSection is JwtIssuerOptions)
            {
                await _JwtOpts.Update(s =>
                {
                    var opts = request.ConfigSection as JwtIssuerOptions;
                    s.Audience = opts.Audience;
                    s.SecretKey = opts.SecretKey;
                    s.Issuer = opts.Issuer;
                    s.ValidFor = opts.ValidFor;
                });
            }
            else if (request.ConfigSection is WebAppInfo)
            {
                await _appInfo.Update(s =>
                {
                    var settings = request.ConfigSection as WebAppInfo;
                    s.Title = settings.Title;
                    s.Description = settings.Description;
                    s.Keywords = settings.Keywords;
                });
            }
            else if (request.ConfigSection is SocialAuthSettings)
            {
                await _socialSettings.Update(s =>
                {
                    var settings = request.ConfigSection as SocialAuthSettings;
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
