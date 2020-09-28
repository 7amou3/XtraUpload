using System.Collections.Generic;
using XtraUpload.Domain;

namespace XtraUpload.WebApi
{
    internal class DeleteFolderResultDto
    {
        public IEnumerable<FolderItem> Folders { get; set; }
        public IEnumerable<FileItemHeaderDto> Files { get; set; }
    }
}
