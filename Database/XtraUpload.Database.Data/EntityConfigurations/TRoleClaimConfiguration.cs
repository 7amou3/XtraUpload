using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XtraUpload.Domain;

namespace XtraUpload.Database.Data
{
    /// <summary>
    /// RoleClaims table configuration
    /// </summary>
    public class TRoleClaimConfiguration : IEntityTypeConfiguration<RoleClaim>
    {
        public void Configure(EntityTypeBuilder<RoleClaim> builder)
        {
            builder.HasKey(u => u.Id);
            
            builder.HasOne(s => s.Role).WithMany(s => s.RoleClaims).HasForeignKey(s => s.RoleId).OnDelete(DeleteBehavior.Cascade);

            int i = 1;
            builder.HasData(
                // Admin claims
                new RoleClaim() { Id = i++, RoleId = "1", ClaimType = XtraUploadClaims.AdminAreaAccess.ToString(), ClaimValue = "1" },
                new RoleClaim() { Id = i++, RoleId = "1", ClaimType = XtraUploadClaims.FileManagerAccess.ToString(), ClaimValue = "1" },
                new RoleClaim() { Id = i++, RoleId = "1", ClaimType = XtraUploadClaims.StorageSpace.ToString(), ClaimValue = "10000" },
                new RoleClaim() { Id = i++, RoleId = "1", ClaimType = XtraUploadClaims.FileExpiration.ToString(), ClaimValue = "0" },
                new RoleClaim() { Id = i++, RoleId = "1", ClaimType = XtraUploadClaims.DownloadTTW.ToString(), ClaimValue = "10" },
                new RoleClaim() { Id = i++, RoleId = "1", ClaimType = XtraUploadClaims.ConcurrentUpload.ToString(), ClaimValue = "10" },
                new RoleClaim() { Id = i++, RoleId = "1", ClaimType = XtraUploadClaims.DownloadSpeed.ToString(), ClaimValue = "2048" },
                new RoleClaim() { Id = i++, RoleId = "1", ClaimType = XtraUploadClaims.MaxFileSize.ToString(), ClaimValue = "2000" },
                new RoleClaim() { Id = i++, RoleId = "1", ClaimType = XtraUploadClaims.WaitTime.ToString(), ClaimValue = "5" },
                // User Claims         
                new RoleClaim() { Id = i++, RoleId = "2", ClaimType = XtraUploadClaims.FileManagerAccess.ToString(), ClaimValue = "1" },
                new RoleClaim() { Id = i++, RoleId = "2", ClaimType = XtraUploadClaims.StorageSpace.ToString(), ClaimValue = "5000" },
                new RoleClaim() { Id = i++, RoleId = "2", ClaimType = XtraUploadClaims.FileExpiration.ToString(), ClaimValue = "30" },
                new RoleClaim() { Id = i++, RoleId = "2", ClaimType = XtraUploadClaims.DownloadTTW.ToString(), ClaimValue = "60" },
                new RoleClaim() { Id = i++, RoleId = "2", ClaimType = XtraUploadClaims.ConcurrentUpload.ToString(), ClaimValue = "5" },
                new RoleClaim() { Id = i++, RoleId = "2", ClaimType = XtraUploadClaims.DownloadSpeed.ToString(), ClaimValue = "1024" },
                new RoleClaim() { Id = i++, RoleId = "2", ClaimType = XtraUploadClaims.MaxFileSize.ToString(), ClaimValue = "500" },
                new RoleClaim() { Id = i++, RoleId = "2", ClaimType = XtraUploadClaims.WaitTime.ToString(), ClaimValue = "10" },
                // Guest Claims        i++
                new RoleClaim() { Id = i++, RoleId = "3", ClaimType = XtraUploadClaims.DownloadTTW.ToString(), ClaimValue = "300" },
                new RoleClaim() { Id = i++, RoleId = "3", ClaimType = XtraUploadClaims.DownloadSpeed.ToString(), ClaimValue = "500" },
                new RoleClaim() { Id = i++, RoleId = "3", ClaimType = XtraUploadClaims.WaitTime.ToString(), ClaimValue = "60" }
                );

            builder.ToTable("RoleClaims");
        }
    }
}
