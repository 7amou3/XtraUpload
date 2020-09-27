using MediatR;
using XtraUpload.Domain;

namespace XtraUpload.FileManager.Service.Common
{
    /// <summary>
    /// Save the file to db
    /// </summary>
    public class SaveFileCommand : IRequest<CreateFileResult>
    {
        public SaveFileCommand(FileItem file)
        {
            File = file;
        }
        public FileItem File { get; }
    }
}
