using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;
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

            var status = file.Status switch
            {
                Protos.ItemStatus.Visible => Domain.ItemStatus.Visible,
                Protos.ItemStatus.Hidden => Domain.ItemStatus.Hidden,
                Protos.ItemStatus.ToBeProcessed => Domain.ItemStatus.To_Be_Processed,
                Protos.ItemStatus.ToBeDeleted => Domain.ItemStatus.To_Be_Deleted,
                _ => Domain.ItemStatus.Visible,
            };
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
                Status = status,
                StorageServerId = Guid.Parse(file.StorageServerId),
            };
        }
        // Domain to proto
        public static gFileItem Convert(this FileItem file)
        {
            if (file == null) return new gFileItem();

            var status = file.Status switch
            {
                Domain.ItemStatus.Visible => Protos.ItemStatus.Visible,
                Domain.ItemStatus.Hidden => Protos.ItemStatus.Hidden,
                Domain.ItemStatus.To_Be_Processed => Protos.ItemStatus.ToBeProcessed,
                Domain.ItemStatus.To_Be_Deleted => Protos.ItemStatus.ToBeDeleted,
                _ => Protos.ItemStatus.Visible,
            };
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
                Status = status,
                StorageServerId = file.StorageServerId.ToString(),
                StorageServer = file.StorageServer.Convert()
            };
        }

        public static IEnumerable<gFileItem> Convert(this IEnumerable<FileItem> files)
        {
            HashSet<gFileItem> gFiles = new HashSet<gFileItem>();
            foreach (var file in files)
            {
                gFiles.Add(file.Convert());
            }
            return gFiles;
        }

        public static gStorageServer Convert(this StorageServer server)
        {
            if (server == null) return null;

            return new gStorageServer()
            {
                Address = server.Address,
                Id = server.Id.ToString()
            };
        }
    }
}
