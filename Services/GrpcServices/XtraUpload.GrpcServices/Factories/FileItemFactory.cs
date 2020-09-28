using Google.Protobuf.WellKnownTypes;
using System;
using XtraUpload.Domain;
using XtraUpload.Protos;

namespace XtraUpload.GrpcServices
{
    public static class FileItemFactory
    {
        // Proto to domain
        public static FileItem Convert(this gFileItem file)
        {
            if (file == null) return new FileItem();

            return new FileItem()
            {
                Id = file.Id,
                TusId = file.TusId,
                UserId = file.UserId,
                Size = file.Size,
                Name = file.Name,
                MimeType = file.MimeType,
                FolderId = file.FolderId,
                Extension = file.Extension,
                CreatedAt = file.CreatedAt.ToDateTime(),
                LastModified = file.LastModified.ToDateTime(),
                IsAvailableOnline = file.IsAvailableOnline,
                StorageServerId = Guid.Parse(file.StorageServerId),
            };
        }
        // Domain to proto
        public static gFileItem Convert(this FileItem file)
        {
            if (file == null) return new gFileItem();

            return new gFileItem()
            {
                Id = file.Id,
                TusId = file.TusId,
                UserId = file.UserId,
                Name = file.Name,
                MimeType = file.MimeType,
                FolderId = file.FolderId,
                Extension = file.Extension,
                Size = uint.Parse(file.Size.ToString()),
                CreatedAt = Timestamp.FromDateTime(DateTime.SpecifyKind(file.CreatedAt, DateTimeKind.Utc)),
                LastModified = Timestamp.FromDateTime(DateTime.SpecifyKind(file.LastModified, DateTimeKind.Utc)),
                IsAvailableOnline = file.IsAvailableOnline,
                StorageServerId = file.StorageServerId.ToString(),
            };
        }
    }
}
