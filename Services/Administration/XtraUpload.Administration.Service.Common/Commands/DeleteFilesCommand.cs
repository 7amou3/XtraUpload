using MediatR;
using System.Collections.Generic;

namespace XtraUpload.Administration.Service.Common
{
    /// <summary>
    /// Delete file(s) by id
    /// </summary>
    public class DeleteFilesCommand : IRequest<DeleteFilesResult>
    {
        public DeleteFilesCommand(IEnumerable<string> filesId)
        {
            FilesId = new HashSet<string>(filesId);
        }
        public IEnumerable<string> FilesId { get; }
    }
}
