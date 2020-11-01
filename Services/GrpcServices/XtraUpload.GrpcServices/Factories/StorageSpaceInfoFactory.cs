using XtraUpload.Domain;
using XtraUpload.Protos;

namespace XtraUpload.GrpcServices
{
    public static class StorageSpaceInfoFactory
    {
        public static StorageSpaceInfo Convert(this gStorageSpaceInfo storageInfo)
        {
            if (storageInfo == null) return null;

            return new StorageSpaceInfo
            {
                FreeDiskSpace = storageInfo.FreeDiskSpace,
                UsedDiskSpace = storageInfo.UsedDiskSpace
            };
        }
    }
}
