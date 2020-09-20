using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XtraUpload.Domain;

namespace XtraUpload.Database.Data
{
    /// <summary>
    /// StorageServer table configuration
    /// </summary>
    class TStorageServerConfiguration : IEntityTypeConfiguration<StorageServer>
    {
        public void Configure(EntityTypeBuilder<StorageServer> builder)
        {
            builder.HasKey(s => s.Id);
            // Index
            builder.HasIndex(u => u.IpAddress).HasName("IpAddress").IsUnique();
            // Each SS have one entry in the File join table
            builder.HasMany(s => s.Files).WithOne(e => e.StorageServer).HasForeignKey(ur => ur.StorageServerId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
