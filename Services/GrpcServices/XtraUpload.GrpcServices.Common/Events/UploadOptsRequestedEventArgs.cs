using System;

namespace XtraUpload.GrpcServices.Common
{
    public class UploadOptsRequestedEventArgs : EventArgs
    {
        public UploadOptsRequestedEventArgs(string serverAddress)
        {
            ServerAddress = serverAddress;
        }
        public string ServerAddress { get; }
    }
}
