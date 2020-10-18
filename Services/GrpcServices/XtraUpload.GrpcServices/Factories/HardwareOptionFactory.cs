using System;
using System.Collections.Generic;
using System.Text;
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
    }
}
