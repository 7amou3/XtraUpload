using XtraUpload.Domain;
using XtraUpload.Protos;

namespace XtraUpload.GrpcServices
{
    public static class HardwareOptionFactory
    {
        public static HardwareCheckOptions Convert(this gHardwareOptions opts)
        {
            if (opts == null) return null;

            return new HardwareCheckOptions()
            {
                MemoryThreshold = (ushort) opts.MemoryThreshold,
                StorageThreshold = (ushort) opts.StorageThreshold
            };
        }

        public static gHardwareOptions Convert(this HardwareCheckOptions opts)
        {
            if (opts == null) return null;

            return new gHardwareOptions()
            {
                MemoryThreshold = opts.MemoryThreshold,
                StorageThreshold = opts.StorageThreshold
            };
        }
    }
}
