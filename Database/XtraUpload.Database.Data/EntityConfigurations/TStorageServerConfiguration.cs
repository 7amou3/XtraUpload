using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
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
            // Let Ef generate ids
            builder.Property(s => s.Id).ValueGeneratedOnAdd();
            // Index
            builder.HasIndex(u => u.Address).HasDatabaseName("IpAddress").IsUnique();
            // Each SS has many entries in the Files table
            builder.HasMany(s => s.Files).WithOne(e => e.StorageServer).HasForeignKey(ur => ur.StorageServerId).OnDelete(DeleteBehavior.Cascade);

            StorageServer server = new StorageServer()
            {
                Id = Guid.NewGuid(),
                State = ServerState.Active,
                Address = "https://localhost:5002"
            };
            builder.HasData(server);

            // Maps to the Users table
            builder.ToTable("storageservers");
        }
    }
}
