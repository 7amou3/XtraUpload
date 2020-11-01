using System;
using XtraUpload.Domain;

namespace XtraUpload.GrpcServices.Common
{
    public class WriteHardwareOptionsEventArgs : EventArgs
    {
        public WriteHardwareOptionsEventArgs(HardwareCheckOptions hardwareOpt, string serverAddress)
        {
            HardwareOpts = hardwareOpt;
            ServerAddress = serverAddress;
        }
        public HardwareCheckOptions HardwareOpts { get; set; }
        public string ServerAddress { get; }
    }
}
