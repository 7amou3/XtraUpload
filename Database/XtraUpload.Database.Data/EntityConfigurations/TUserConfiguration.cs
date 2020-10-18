using XtraUpload.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XtraUpload.Domain.Infra;
using System;
using System.Collections.Generic;

namespace XtraUpload.Database.Data
{
    /// <summary>
    /// Users table configuration
    /// </summary>
    public class TUserConfiguration : IEntityTypeConfiguration<User>
    {
        private readonly EntityTypeBuilder<FolderItem> _tfolder;
        public TUserConfiguration(EntityTypeBuilder<FolderItem> tfolder)
        {
            _tfolder = tfolder;
        }
        public void Configure(EntityTypeBuilder<User> builder)
        {
            // Primary key
            builder.HasKey(u => u.Id);
            // Indexes for "email" to allow efficient lookups
            builder.HasIndex(u => u.Email).HasName("Email").IsUnique();

            // Limit the size of columns to use efficient database types
            builder.Property(u => u.UserName).HasMaxLength(255);
            builder.Property(u => u.Email).HasMaxLength(255);

            // Each User have one entry in the Role join table
            builder.HasOne(u => u.Role).WithMany(e => e.Users).HasForeignKey(ur => ur.RoleId).OnDelete(DeleteBehavior.Cascade);
            // Each user can have many Files
            builder.HasMany(u => u.Files).WithOne(u => u.User).HasForeignKey(ur => ur.UserId).OnDelete(DeleteBehavior.SetNull);
            // Each user can have many Folders
            builder.HasMany(u => u.Folders).WithOne(u => u.User).HasForeignKey(ur => ur.UserId).OnDelete(DeleteBehavior.Cascade);
            // Each user can have many confirmationtokens
            builder.HasMany(u => u.ConfirmationKeys).WithOne(u => u.User).HasForeignKey(ur => ur.UserId).OnDelete(DeleteBehavior.Cascade);

            User admin = new User()
            {
                UserName = "Admin",
                Email = "admin@admin.com",
                Password = Helpers.HashPassword("admin01"),
                CreatedAt = DateTime.Now,
                LastModified = DateTime.Now,
                RoleId = "1", // admin rol id
                Theme = Theme.Light
            };
            // Add admin
            builder.HasData(admin);

            // Add his folders
            IEnumerable<FolderItem> folders = Helpers.GenerateDefaultFolders(admin.Id);
            _tfolder.HasData(folders);

            // Maps to the Users table
            builder.ToTable("Users");
        }
    }
}
