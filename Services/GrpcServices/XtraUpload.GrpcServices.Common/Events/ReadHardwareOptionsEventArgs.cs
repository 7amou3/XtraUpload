using System;

namespace XtraUpload.GrpcServices.Common
{
    public class ReadHardwareOptionsEventArgs : EventArgs
    {
        public ReadHardwareOptionsEventArgs(string serverAddress)
        {
            ServerAddress = serverAddress;
        }
        public string ServerAddress { get; }
    }
}
