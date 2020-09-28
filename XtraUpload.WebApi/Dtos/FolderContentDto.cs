using System.Collections.Generic;

namespace XtraUpload.WebApi
{
    internal class FolderContentDto
    {
        public IEnumerable<FolderItemDto> Folders { get; set; }
        public IEnumerable<FileItemDto> Files { get; set; }
    }
}
