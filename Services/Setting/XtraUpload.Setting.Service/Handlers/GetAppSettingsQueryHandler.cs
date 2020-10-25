using MediatR;
using Microsoft.Extensions.Options;
using System.Threading;
using System.Threading.Tasks;
using XtraUpload.Authentication.Service.Common;
using XtraUpload.Domain;
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
        readonly IOptionsMonitor<WebAppInfo> _appInfo;
        readonly IOptionsMonitor<SocialAuthSettings> _socialSettings;

        public GetAppSettingsQueryHandler(
            IOptionsMonitor<JwtIssuerOptions> jwtOpts,
            IOptionsMonitor<EmailSettings> emailSettings,
            IOptionsMonitor<HardwareCheckOptions> hdOpts,
            IOptionsMonitor<WebAppInfo> appInfo, 
            IOptionsMonitor<SocialAuthSettings> socialSettings)
        {
            _hdOpts = hdOpts;
            _JwtOpts = jwtOpts;
            _appInfo = appInfo;
            _emailSettings = emailSettings;
            _socialSettings = socialSettings;
        }

        public Task<ReadAppSettingResult> Handle(GetAppSettingsQuery request, CancellationToken cancellationToken)
        {
            ReadAppSettingResult settings = new ReadAppSettingResult()
            {
                AppInfo = _appInfo.CurrentValue,
                EmailSettings = _emailSettings.CurrentValue,
                HardwareCheckOptions = _hdOpts.CurrentValue,
                JwtIssuerOptions = _JwtOpts.CurrentValue,
                SocialAuthSettings = _socialSettings.CurrentValue
            };
            return Task.FromResult(settings);
        }

    }
}
