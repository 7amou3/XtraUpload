using System;
using XtraUpload.Domain;

namespace XtraUpload.GrpcServices.Common
{
    public class WriteUploadOptsRequestedEventArgs : EventArgs
    {
        public WriteUploadOptsRequestedEventArgs(UploadOptions uploadOptions, string serverAddress)
        {
            UploadOptions = uploadOptions;
            ServerAddress = serverAddress;
        }
        public UploadOptions UploadOptions { get; }
        public string ServerAddress { get; }
    }
}
