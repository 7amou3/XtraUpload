using System.Collections.Generic;

namespace XtraUpload.WebApp
{
    internal class DeleteItemsResultDto
    {
        public IEnumerable<DeleteFolderResultDto> Folders { get; set; }
        public IEnumerable<FileItemHeaderDto> Files { get; set; }
    }
}
