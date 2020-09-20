using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XtraUpload.Domain;

namespace XtraUpload.Database.Data
{
    /// <summary>
    /// FileExtensions table configuration
    /// </summary>
    public class TFileExtensionConfiguration : IEntityTypeConfiguration<FileExtension>
    {
        public void Configure(EntityTypeBuilder<FileExtension> builder)
        {
            builder.HasKey(u => u.Id);
            // Add index
            builder.HasIndex(u => u.Name).HasName("Name").IsUnique();
            // Limit the size of columns to use efficient database types
            builder.Property(u => u.Name).HasMaxLength(8);
            int i = 1;
            // Seed table
            builder.HasData(
                new FileExtension() { Id = i++, Name = ".doc" },
                new FileExtension() { Id = i++, Name = ".docx" },
                new FileExtension() { Id = i++, Name = ".odt" },
                new FileExtension() { Id = i++, Name = ".rtf" },
                new FileExtension() { Id = i++, Name = ".tex" },
                new FileExtension() { Id = i++, Name = ".txt" },
                new FileExtension() { Id = i++, Name = ".log" },
                new FileExtension() { Id = i++, Name = ".csv" },
                new FileExtension() { Id = i++, Name = ".ppt" },
                new FileExtension() { Id = i++, Name = ".pptx" },
                new FileExtension() { Id = i++, Name = ".xml" },
                new FileExtension() { Id = i++, Name = ".pdf" },
                new FileExtension() { Id = i++, Name = ".xls" },
                new FileExtension() { Id = i++, Name = ".xlsx" },
                new FileExtension() { Id = i++, Name = ".mp3" },
                new FileExtension() { Id = i++, Name = ".wav" },
                new FileExtension() { Id = i++, Name = ".wma" },
                new FileExtension() { Id = i++, Name = ".png" },
                new FileExtension() { Id = i++, Name = ".jpg" },
                new FileExtension() { Id = i++, Name = ".jpe" },
                new FileExtension() { Id = i++, Name = ".jpeg" },
                new FileExtension() { Id = i++, Name = ".rar" },
                new FileExtension() { Id = i++, Name = ".tar.gz" },
                new FileExtension() { Id = i++, Name = ".pkg" },
                new FileExtension() { Id = i++, Name = ".7z" },
                new FileExtension() { Id = i++, Name = ".zip" },
                new FileExtension() { Id = i++, Name = ".tar" },
                new FileExtension() { Id = i++, Name = ".gzip" },
                new FileExtension() { Id = i++, Name = ".iso" },
                new FileExtension() { Id = i++, Name = ".bin" },
                new FileExtension() { Id = i++, Name = ".mdf" },
                new FileExtension() { Id = i++, Name = ".aaf" },
                new FileExtension() { Id = i++, Name = ".mp4" },
                new FileExtension() { Id = i++, Name = ".flv" },
                new FileExtension() { Id = i++, Name = ".mov" },
                new FileExtension() { Id = i++, Name = ".swf" },
                new FileExtension() { Id = i++, Name = ".avi" });
        }
    }
}