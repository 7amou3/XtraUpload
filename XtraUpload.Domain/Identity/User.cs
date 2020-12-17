using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace XtraUpload.Domain
{
    public class User: IdentityUser
    {
        public User()
        {
            Files = new HashSet<FileItem>();
            Folders = new HashSet<FolderItem>();
            ConfirmationKeys = new HashSet<ConfirmationKey>();
        }

        public string Password { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool AccountSuspended { get; set; }
        public Theme Theme { get; set; }
        public DateTime LastModified { get; set; }
        public string SocialMediaId { get; set; }
        public AuthProvider Provider { get; set; }
        public string Avatar { get; set; }

        public virtual string RoleId { get; set; }
        public virtual Role Role { get; set; }
        public virtual Guid LanguageId { get; set; }
        public virtual Language Language { get; set; }
        public virtual ICollection<FileItem> Files { get; set; }
        public virtual ICollection<FolderItem> Folders { get; set; }
        public virtual ICollection<ConfirmationKey> ConfirmationKeys { get; set; }

    }
}
