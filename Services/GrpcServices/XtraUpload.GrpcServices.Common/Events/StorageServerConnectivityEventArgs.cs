using System;

namespace XtraUpload.GrpcServices.Common
{
    public class StorageServerConnectivityEventArgs : EventArgs
    {
        public StorageServerConnectivityEventArgs(string serverAddress)
        {
            ServerAddress = serverAddress;
        }
        public string ServerAddress { get; }
    }
}
