using System;

namespace XtraUpload.GrpcServices.Common
{
    public class ReadUploadOptionsEventArgs : EventArgs
    {
        public ReadUploadOptionsEventArgs(string serverAddress)
        {
            ServerAddress = serverAddress;
        }
        public string ServerAddress { get; }
    }
}
