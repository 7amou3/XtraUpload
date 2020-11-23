using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using XtraUpload.Domain;

namespace XtraUpload.Database.Data
{
    public class TLanguageConfiguration : IEntityTypeConfiguration<Language>
    {
        public void Configure(EntityTypeBuilder<Language> builder)
        {
            // Primary key
            builder.HasKey(p => p.Id);
            // Index
            builder.HasIndex(u => u.Culture).HasDatabaseName("Culture").IsUnique();
            // Fields length
            builder.Property(u => u.Culture).HasMaxLength(8);
            builder.Property(u => u.Name).HasMaxLength(24);
            // Seed table
            builder.HasData(
                new Language() { Id = Guid.NewGuid(), Name = "English", Culture = "en-US" },
                new Language() { Id = Guid.NewGuid(), Name = "Francais", Culture = "fr-FR" },
                new Language() { Id = Guid.NewGuid(), Name = "Español", Culture = "es-ES" },
                new Language() { Id = Guid.NewGuid(), Name = "العربية", Culture = "ar-MA" }
                );
        }
    }
}
