using MediatR;

namespace XtraUpload.Administration.Service.Common
{
    public class UpdateExtensionCommand : IRequest<FileExtensionResult>
    {
        public UpdateExtensionCommand(int id, string name)
        {
            Name = name;
            Id = id;
        }
        public int Id { get; }

        public string Name { get; }
    }
}
