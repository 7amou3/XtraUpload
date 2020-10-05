using MediatR;
using System.Collections.Generic;

namespace XtraUpload.FileManager.Service.Common
{
    /// <summary>
    /// Delete the file from db
    /// </summary>
    public class DeleteFileFromDbCommand : IRequest<DeleteFileResult>
    {
        public DeleteFileFromDbCommand(IEnumerable<string> filesId)
        {
            FilesId = new HashSet<string>(filesId);
        }
        public IEnumerable<string> FilesId { get; set; }
    }
}
