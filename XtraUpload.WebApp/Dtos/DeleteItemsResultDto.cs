using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XtraUpload.WebApp
{
    internal class DeleteItemsResultDto
    {
        public IEnumerable<DeleteFolderResultDto> Folders { get; set; }
        public IEnumerable<MinFileInfoDto> Files { get; set; }
    }
}
