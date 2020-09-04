using MediatR;

namespace XtraUpload.FileManager.Service.Common
{
    /// <summary>
    /// Delete the file from the drive and db
    /// </summary>
    public class DeleteFileCommand : IRequest<DeleteFileResult>
    {
        public DeleteFileCommand(string fileId)
        {
            FileId = fileId;
        }
        public string FileId { get; set; }
    }
}
