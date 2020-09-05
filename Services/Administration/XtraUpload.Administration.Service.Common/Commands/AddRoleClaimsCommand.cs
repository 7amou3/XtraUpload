using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using XtraUpload.Authentication.Service.Common;
using XtraUpload.Domain;

namespace XtraUpload.Administration.Service.Common
{
    /// <summary>
    /// Add a new role claims
    /// </summary>
    public class AddRoleClaimsCommand : IRequest<RoleClaimsResult>
    {
        public Role Role { get; set; }
        public XuClaims Claims { get; set; }
    }

    public class XuClaims
    {
        public bool? AdminAreaAccess { get; set; }
        public bool? FileManagerAccess { get; set; }
        public ushort? ConcurrentUpload { get; set; }
        public ushort? DownloadSpeed { get; set; }
        public ushort? DownloadTTW { get; set; }
        public ushort? FileExpiration { get; set; }
        public ushort? MaxFileSize { get; set; }
        public ushort? StorageSpace { get; set; }
        public ushort? WaitTime { get; set; }
    }
}
