using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XtraUpload.ServerApp.Common
{
    public class ConfirmEmailViewModel
    {
        [Required]
        [MinLength(6)]
        [RegularExpression("^[a-zA-Z0-9]*$")]
        public string EmailToken { get; set; }
    }
}
