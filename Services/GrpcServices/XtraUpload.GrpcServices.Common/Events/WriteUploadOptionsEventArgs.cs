using System;
using XtraUpload.Domain;

namespace XtraUpload.GrpcServices.Common
{
    public class WriteUploadOptionsEventArgs : EventArgs
    {
        public WriteUploadOptionsEventArgs(UploadOptions uploadOptions, string serverAddress)
        {
            UploadOptions = uploadOptions;
            ServerAddress = serverAddress;
        }
        public UploadOptions UploadOptions { get; }
        public string ServerAddress { get; }
    }
}
