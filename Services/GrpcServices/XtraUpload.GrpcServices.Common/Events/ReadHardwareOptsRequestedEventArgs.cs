using System;

namespace XtraUpload.GrpcServices.Common
{
    public class ReadHardwareOptsRequestedEventArgs : EventArgs
    {
        public ReadHardwareOptsRequestedEventArgs(string serverAddress)
        {
            ServerAddress = serverAddress;
        }
        public string ServerAddress { get; }
    }
}
