using System;
using XtraUpload.Authentication.Service.Common;
using XtraUpload.Domain;

namespace XtraUpload.WebApi
{
    internal class UserDto
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool AccountSuspended { get; set; }
        public Theme Theme { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastModified { get; set; }
        public long? FacebookId { get; set; }
        public string Avatar { get; set; }
        public JwtToken JwtToken { get; set; }
        public string Role { get; set; }
        public string Language { get; set; }
    }
}
