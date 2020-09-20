using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using XtraUpload.Domain;

namespace XtraUpload.Database.Data
{
    public class ApplicationDbContext: IdentityDbContext<User, Role, string, IdentityUserClaim<string>, IdentityUserRole<string>, IdentityUserLogin<string>, RoleClaim, IdentityUserToken<string>>
    {
        public ApplicationDbContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<FileItem> Files { get; set; }
        public DbSet<FolderItem> Folders { get; set; }
        public DbSet<Download> Downloads { get; set; }
        public DbSet<ConfirmationKey> ConfirmationKeys { get; set; }
        public DbSet<FileExtension> FileExtensions { get; set; }
        public DbSet<Page> Pages { get; set; }
        public DbSet<StorageServer> StorageServers { get; set; }
        // Identity models are inherited from the base class, no need to redefine them here..

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // for more information on how to configure and customize identity models:
            // https://docs.microsoft.com/en-us/aspnet/core/security/authentication/customize-identity-model?view=aspnetcore-3.1
           
            builder.ApplyConfiguration(new TRoleConfiguration());
            builder.ApplyConfiguration(new TRoleClaimConfiguration());
            builder.ApplyConfiguration(new TUserConfiguration(builder.Entity<FolderItem>()));
            builder.ApplyConfiguration(new TFileExtensionConfiguration());
            builder.ApplyConfiguration(new TPageConfiguration());
            builder.ApplyConfiguration(new TFileConfiguration());
            builder.ApplyConfiguration(new TStorageServerConfiguration());
        }
    }
}
