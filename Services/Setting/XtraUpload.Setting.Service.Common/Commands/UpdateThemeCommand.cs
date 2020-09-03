using MediatR;
using XtraUpload.Domain;

namespace XtraUpload.Setting.Service.Common
{
    public class UpdateThemeCommand : IRequest<OperationResult>
    {
        public Theme Theme { get; set; }
    }
}
