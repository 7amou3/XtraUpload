using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using XtraUpload.Domain;
using XtraUpload.Domain.Infra;

namespace XtraUpload.Database.Data
{
    public class TPageConfiguration : IEntityTypeConfiguration<Page>
    {
        public void Configure(EntityTypeBuilder<Page> builder)
        {
            // Primary key
            builder.HasKey(p => p.Id);
            // Index
            builder.HasIndex(u => u.Name).HasName("Name").IsUnique();
            // Fields length
            builder.Property(p => p.Id).HasMaxLength(20);
            builder.Property(p => p.Name).HasMaxLength(255);
            // Seed
            builder.HasData(new Page()
            {
                Id = Helpers.GenerateUniqueId(),
                Name = "TOS",
                Url = "tos",
                Content = "Term of services content here",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            },
            new Page()
            {
                Id = Helpers.GenerateUniqueId(),
                Name = "Privacy Policy",
                Content = "Privacy Policy content here",
                Url = "privacy_policy",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            },
            new Page()
            {
                Id = Helpers.GenerateUniqueId(),
                Name = "Copyright",
                Url = "copyright",
                Content = "Copyright content here",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            }
            );
        }
    }
}
