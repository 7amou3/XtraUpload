using MediatR;

namespace XtraUpload.Administration.Service.Common
{
    /// <summary>
    /// Add an extension
    /// </summary>
    public class AddExtensionCommand : IRequest<FileExtensionResult>
    {
        public AddExtensionCommand(string extName)
        {
            ExtName = extName;
        }
        public string ExtName { get; set; }
    }
}
