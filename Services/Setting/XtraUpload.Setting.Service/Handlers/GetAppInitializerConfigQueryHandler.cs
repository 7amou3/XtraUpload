using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XtraUpload.Administration.Service.Common;
using XtraUpload.Setting.Service.Common;

namespace XtraUpload.Setting.Service
{
    public class GetAppInitializerConfigQueryHandler : IRequestHandler<GetAppInitializerConfigQuery, AppInitializerConfigResult>
    {
        readonly IMediator _mediatr;
        readonly IOptionsMonitor<WebAppInfo> _appInfo;
        readonly ILogger<GetAppInitializerConfigQueryHandler> _logger;

        public GetAppInitializerConfigQueryHandler(IMediator mediatr, IOptionsMonitor<WebAppInfo> appInfo,
            ILogger<GetAppInitializerConfigQueryHandler> logger)
        {
            _logger = logger;
            _mediatr = mediatr;
            _appInfo = appInfo;
        }

        public async Task<AppInitializerConfigResult> Handle(GetAppInitializerConfigQuery request, CancellationToken cancellationToken)
        {
            AppInitializerConfigResult Result = new AppInitializerConfigResult()
            {
                AppInfo = _appInfo.CurrentValue,
            };
            var pagesResult = await _mediatr.Send(new GetPagesHeaderQuery(s => s.VisibleInFooter == true));
            if (pagesResult.State == Domain.OperationState.Success)
            {
                Result.Pages = pagesResult.PagesHeader;
            }
            else
            {
                _logger.LogError("Error occured while reading application static pages. " + pagesResult.ErrorContent);
                Result = Domain.OperationResult.CopyResult<AppInitializerConfigResult>(pagesResult);
            }
            var Version = Assembly.GetEntryAssembly().GetName().Version;
            // XtraUpload use semantic versioning
            Result.Version = $"{Version.Major}.{Version.Minor}.{Version.Build}";

            return Result;
        }
    }
}
