using System;
using XtraUpload.Domain;

namespace XtraUpload.WebApi
{
    internal class StorageServerDto
    {
        public Guid Id { get; set; }
        public string Address { get; set; }
        public ServerState State { get; set; }
    }
}
