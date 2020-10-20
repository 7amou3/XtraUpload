using System;

namespace XtraUpload.GrpcServices.Common
{
    public class ReadUploadOptsRequestedEventArgs : EventArgs
    {
        public ReadUploadOptsRequestedEventArgs(string serverAddress)
        {
            ServerAddress = serverAddress;
        }
        public string ServerAddress { get; }
    }
}
