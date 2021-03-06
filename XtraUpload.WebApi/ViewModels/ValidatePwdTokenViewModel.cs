﻿using System.ComponentModel.DataAnnotations;

namespace XtraUpload.WebApi
{
    public class ValidatePwdTokenViewModel
    {
        [Required]
        [MinLength(6)]
        public string NewPassword { get; set; }

        [Required]
        [MinLength(6)]
        [RegularExpression("^[a-zA-Z0-9]*$")]
        public string RecoveryKey { get; set; }
    }
}
