using System;
using System.Collections.Generic;
using System.Text;

namespace XtraUpload.GrpcServices.Common
{
    public class ReadStorageHealthEventArgs : EventArgs
    {
        public ReadStorageHealthEventArgs(string serverAddress)
        {
            ServerAddress = serverAddress;
        }
        public string ServerAddress { get; }
    }
}
