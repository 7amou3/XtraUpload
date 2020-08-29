using System;
using System.Collections.Generic;
using System.Text;
using XtraUpload.Domain;

namespace XtraUpload.Administration.Service.Common
{
    public class PagingResult<T> : OperationResult
    {
        public IEnumerable<T> Items { get; set; }
        public int TotalItems { get; set; }
    }

    /// <summary>
    /// User associated with it's role name
    /// </summary>
    public class UserExtended : User
    {
        public string RoleName { get; set; }
    }
    /// <summary>
    /// File associated with it's user name
    /// </summary>
    public class FileItemExtended : FileItem
    {
        public string UserName { get; set; }
    }
}
