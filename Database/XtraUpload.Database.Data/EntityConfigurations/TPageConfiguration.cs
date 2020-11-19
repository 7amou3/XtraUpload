using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using XtraUpload.Domain;

namespace XtraUpload.Database.Data
{
    public class TPageConfiguration : IEntityTypeConfiguration<Page>
    {
        public void Configure(EntityTypeBuilder<Page> builder)
        {
            // Primary key
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).ValueGeneratedNever();
            // Index
            builder.HasIndex(u => u.Name).HasDatabaseName("Name").IsUnique();
            // Fields length
            builder.Property(p => p.Id).HasMaxLength(20);
            builder.Property(p => p.Name).HasMaxLength(255);
            // Seed
            builder.HasData(new Page()
            {
                Id = Helpers.GenerateUniqueId(),
                Name = "Terms of service",
                Url = "terms_of_service",
                Content = "Terms of service content here",
                VisibleInFooter = true,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            },
            new Page()
            {
                Id = Helpers.GenerateUniqueId(),
                Name = "Privacy Policy",
                Content = "Privacy Policy content here",
                Url = "privacy_policy",
                VisibleInFooter = true,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            },
            new Page()
            {
                Id = Helpers.GenerateUniqueId(),
                Name = "Copyright",
                Url = "copyright",
                VisibleInFooter = true,
                Content = "Copyright content here",
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            }
            );
        }
    }
}
