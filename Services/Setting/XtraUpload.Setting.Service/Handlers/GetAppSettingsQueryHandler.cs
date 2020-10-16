using MediatR;
using Microsoft.Extensions.Options;
using System.Threading;
using System.Threading.Tasks;
using XtraUpload.Authentication.Service.Common;
using XtraUpload.Email.Service.Common;
using XtraUpload.Setting.Service.Common;

namespace XtraUpload.Setting.Service
{
    /// <summary>
    /// Read config file
    /// </summary>
    public class GetAppSettingsQueryHandler : IRequestHandler<GetAppSettingsQuery, ReadAppSettingResult>
    {
        readonly IOptionsMonitor<JwtIssuerOptions> _JwtOpts;
        readonly IOptionsMonitor<EmailSettings> _emailSettings;
        readonly IOptionsMonitor<HardwareCheckOptions> _hdOpts;
        readonly IOptionsMonitor<WebAppSettings> _appSettings;
        readonly IOptionsMonitor<SocialAuthSettings> _socialSettings;

        public GetAppSettingsQueryHandler(
            IOptionsMonitor<JwtIssuerOptions> jwtOpts,
            IOptionsMonitor<EmailSettings> emailSettings,
            IOptionsMonitor<HardwareCheckOptions> hdOpts,
            IOptionsMonitor<WebAppSettings> appSettings, 
            IOptionsMonitor<SocialAuthSettings> socialSettings)
        {
            _hdOpts = hdOpts;
            _JwtOpts = jwtOpts;
            _appSettings = appSettings;
            _emailSettings = emailSettings;
            _socialSettings = socialSettings;
        }

        public Task<ReadAppSettingResult> Handle(GetAppSettingsQuery request, CancellationToken cancellationToken)
        {
            ReadAppSettingResult settings = new ReadAppSettingResult()
            {
                AppSettings = _appSettings.CurrentValue,
                EmailSettings = _emailSettings.CurrentValue,
                HardwareCheckOptions = _hdOpts.CurrentValue,
                JwtIssuerOptions = _JwtOpts.CurrentValue,
                SocialAuthSettings = _socialSettings.CurrentValue
            };
            return Task.FromResult(settings);
        }

    }
}
