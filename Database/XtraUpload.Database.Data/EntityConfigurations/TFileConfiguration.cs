using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using XtraUpload.Domain;

namespace XtraUpload.Database.Data
{
    /// <summary>
    /// Files table configuration
    /// </summary>
    public class TFileConfiguration : IEntityTypeConfiguration<FileItem>
    {
        public void Configure(EntityTypeBuilder<FileItem> builder)
        {
            builder.HasKey(s => s.Id);
            // Limit the size of columns to use efficient database types
            builder.Property(u => u.Extension).HasMaxLength(6);
            builder.Property(u => u.Extension).HasMaxLength(8);
            builder.Property(u => u.Name).HasMaxLength(255);

            // Each file can have many downloads
            builder.HasMany(d => d.Downloads).WithOne(f => f.File).HasForeignKey(ur => ur.FileId).OnDelete(DeleteBehavior.Cascade);
            // Each file have one storage server
            builder.HasOne(s => s.StorageServer).WithMany(s => s.Files).HasForeignKey(s => s.StorageServerId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
