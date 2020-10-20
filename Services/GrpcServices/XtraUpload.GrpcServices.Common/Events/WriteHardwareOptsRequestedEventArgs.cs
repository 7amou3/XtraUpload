using System;
using XtraUpload.Domain;

namespace XtraUpload.GrpcServices.Common
{
    public class WriteHardwareOptsRequestedEventArgs : EventArgs
    {
        public WriteHardwareOptsRequestedEventArgs(HardwareCheckOptions hardwareOpt, string serverAddress)
        {
            HardwareOpts = hardwareOpt;
            ServerAddress = serverAddress;
        }
        public HardwareCheckOptions HardwareOpts { get; set; }
        public string ServerAddress { get; }
    }
}
