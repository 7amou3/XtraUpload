using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XtraUpload.WebApp.Common
{
    public class PublicFolderViewModel
    {
        [Required]
        [RegularExpression("^[a-zA-Z0-9]*$")]
        public string MainFolderId { get; set; }

        [RegularExpression("^[a-zA-Z0-9]*$")]
        public string ChildFolderId { get; set; }
    }
}
