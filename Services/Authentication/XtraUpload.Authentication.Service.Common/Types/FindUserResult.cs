using System;
using System.Collections.Generic;
using System.Text;
using XtraUpload.Domain;

namespace XtraUpload.Authentication.Service.Common
{
    public class FindUserResult: OperationResult
    {
        public User User { get; set; }
    }
}
