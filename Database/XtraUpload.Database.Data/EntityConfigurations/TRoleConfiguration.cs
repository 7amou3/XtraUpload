using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XtraUpload.Domain;

namespace XtraUpload.Database.Data
{
    /// <summary>
    /// Roles table configuration
    /// </summary>
    class TRoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.HasKey(u => u.Id);
            builder.HasData(
                new Role() { Name = "Admin", Id = "1", IsDefault = true },
                new Role() { Name = "User", Id = "2", IsDefault = true },
                new Role() { Name = "Guest", Id = "3", IsDefault = true }
               );
            builder.ToTable("Roles");
        }
    }
}
