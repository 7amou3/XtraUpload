using System;

namespace XtraUpload.GrpcServices.Common
{
    public class HardwareOptsRequestedEventArgs : EventArgs
    {
        public HardwareOptsRequestedEventArgs(string serverAddress)
        {
            ServerAddress = serverAddress;
        }
        public string ServerAddress { get; }
    }
}
