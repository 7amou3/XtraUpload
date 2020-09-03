using MediatR;
using XtraUpload.Domain;

namespace XtraUpload.Setting.Service.Common
{
    public class UpdateConfigSectionCommand : IRequest<OperationResult>
    {
        public UpdateConfigSectionCommand(object configSection)
        {
            ConfigSection = configSection;
        }
        public object ConfigSection { get; }
    }
}
