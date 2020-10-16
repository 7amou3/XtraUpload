using XtraUpload.Domain;
using XtraUpload.Protos;

namespace XtraUpload.GrpcServices
{
    public static class UploadOptionFactory
    {
        public static UploadOptions Convert(this gUploadOptions opts)
        {
            if (opts == null) return null;

            return new UploadOptions()
            {
                UploadPath = opts.UploadPath,
                ChunkSize = opts.ChunkSize,
                Expiration = opts.Expiration
            };
        }
    }
}
