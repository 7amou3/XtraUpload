using System.Collections.Generic;

namespace XtraUpload.WebApi
{
    internal class DeleteItemsResultDto
    {
        public IEnumerable<DeleteFolderResultDto> Folders { get; set; }
        public IEnumerable<FileItemHeaderDto> Files { get; set; }
    }
}
