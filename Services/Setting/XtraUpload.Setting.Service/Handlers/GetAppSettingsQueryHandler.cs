using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XtraUpload.Authentication.Service.Common;
using XtraUpload.Domain;
using XtraUpload.Email.Service.Common;
using XtraUpload.FileManager.Service.Common;
using XtraUpload.Setting.Service.Common;

namespace XtraUpload.Setting.Service
{
    public class GetAppSettingsQueryHandler : IRequestHandler<GetAppSettingsQuery, ReadAppSettingResult>
    {
        #region Fields
        readonly IWritableOptions<JwtIssuerOptions> _JwtOpts;
        readonly IWritableOptions<UploadOptions> _uploadOpts;
        readonly IWritableOptions<EmailSettings> _emailSettings;
        readonly IWritableOptions<HardwareCheckOptions> _hdOpts;
        readonly IWritableOptions<WebAppSettings> _appSettings;
        readonly IWritableOptions<SocialAuthSettings> _socialSettings;
        #endregion

        #region Constructor
        public GetAppSettingsQueryHandler(IWritableOptions<JwtIssuerOptions> jwtOpts, IWritableOptions<UploadOptions> uploadOpts,
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

        #endregion

        #region Handler
        public Task<ReadAppSettingResult> Handle(GetAppSettingsQuery request, CancellationToken cancellationToken)
        {
            ReadAppSettingResult settings = new ReadAppSettingResult()
            {
                AppSettings = _appSettings.Value,
                EmailSettings = _emailSettings.Value,
                HardwareCheckOptions = _hdOpts.Value,
                JwtIssuerOptions = _JwtOpts.Value,
                UploadOptions = _uploadOpts.Value,
                SocialAuthSettings = _socialSettings.Value
            };
            return Task.FromResult(settings);
        }

        #endregion
    }
}
