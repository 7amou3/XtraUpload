﻿using System.Collections.Generic;
using XtraUpload.Domain;

namespace XtraUpload.FileManager.Service.Common
{
    public class DeleteFileResult: OperationResult
    {
        public IEnumerable<FileItem> Files { get; set; }
    }
}
