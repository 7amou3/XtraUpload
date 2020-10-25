using MediatR;
using XtraUpload.Administration.Service.Common;

namespace XtraUpload.Setting.Service.Common
{
    public class GetPageQuery : IRequest<PageResult>
    {
        public GetPageQuery(string pageUrl)
        {
            PageUrl = pageUrl;
        }
        public string PageUrl { get; }
    }
}
