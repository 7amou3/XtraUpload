using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using XtraUpload.GrpcServices.Common;
using XtraUpload.Protos;

namespace XtraUpload.GrpcServices
{
    /// <summary>
    /// Service definition for storage service, server config, monitoring state...
    /// </summary>
    [Authorize(AuthenticationSchemes = CertificateAuthenticationDefaults.AuthenticationScheme)]
    public class gStorageManagerService : gStorageManager.gStorageManagerBase
    {
        readonly ICheckClientProxy _checkClientProxy;
        readonly IHardwareOptsClientCommand _hardwareOptsCmd;
        readonly IUploadOptsClientCommand _uploadOptsCommand;
        readonly ILogger<gStorageManagerService> _logger;
        public gStorageManagerService(
            ICheckClientProxy checkClientProxy,
            IUploadOptsClientProxy uploadOptsProxy,
            IHardwareOptsClientProxy hardwareProxy,
            ILogger<gStorageManagerService> logger)
        {
            _logger = logger;
            _checkClientProxy = checkClientProxy;
            _hardwareOptsCmd = hardwareProxy as IHardwareOptsClientCommand;
            _uploadOptsCommand = uploadOptsProxy as IUploadOptsClientCommand;

        }
        public override Task<AuthResponse> IsAuthorized(Empty request, ServerCallContext context)
        {
            var authorized = context.GetHttpContext().User.Identity.IsAuthenticated;
            var response = new AuthResponse()
            {
                Status = new gRequestStatus()
                {
                    Status = authorized ? RequestStatus.Success : Protos.RequestStatus.Failed
                }
            };
            return Task.FromResult(response);
        }
        public override async Task CheckConnectivity(IAsyncStreamReader<ConnectivityResponse> requestStream, IServerStreamWriter<ConnectivityRequest> responseStream, ServerCallContext context)
        {
            _checkClientProxy.StorageServerConnectivityRequested += ConnectivityRequested;
            try
            {
                await foreach (var message in requestStream.ReadAllAsync())
                {
                    string server = context.Host;
                    _logger.LogDebug("Request received from " + server);

                    _checkClientProxy.SetConnectivityStatus(message.Status.Convert());
                }
            }
            finally
            {
                _logger.LogError("Connection lost with the remote storage server");
                _checkClientProxy.StorageServerConnectivityRequested -= ConnectivityRequested;
            }
            
            async void ConnectivityRequested(object sender, StorageServerConnectivityEventArgs e)
            {
                _logger.LogDebug("Sending request to " + e.ServerAddress);

                await responseStream.WriteAsync(new ConnectivityRequest() { ServerAddress = e.ServerAddress });
            }
        }

        public override async Task GetUploadOptions(IAsyncStreamReader<UploadOptsResponse> requestStream, IServerStreamWriter<UploadOptsRequest> responseStream, ServerCallContext context)
        {
            _uploadOptsCommand.ReadUploadOptsRequested += uploadOptionsRequested;
            try
            {
                await foreach (var message in requestStream.ReadAllAsync())
                {
                    _logger.LogDebug("Request received from " + message.ServerAddress);

                    _uploadOptsCommand.SetUploadOptions(message.UploadOptions.Convert(), message.ServerAddress);
                }
                
            }
            finally
            {
                _logger.LogError("Connection lost with the remote storage server");
                _uploadOptsCommand.ReadUploadOptsRequested -= uploadOptionsRequested;
            }

            async void uploadOptionsRequested(object sender, ReadUploadOptsRequestedEventArgs e)
            {
                _logger.LogDebug("Sending request to " + e.ServerAddress);

                await responseStream.WriteAsync(new UploadOptsRequest() { ServerAddress = e.ServerAddress });
            }
        }
        public override async Task SetUploadOptions(IAsyncStreamReader<UploadOptsResponse> requestStream, IServerStreamWriter<UploadOptsResponse> responseStream, ServerCallContext context)
        {
            _uploadOptsCommand.WriteUploadOptsRequested += uploadOptionsRequested;
            try
            {
                await foreach (var message in requestStream.ReadAllAsync())
                {
                    _logger.LogDebug("Request received from " + message.ServerAddress);

                    _uploadOptsCommand.SetUploadOptions(message.UploadOptions.Convert(), message.ServerAddress);
                }
            }
            finally
            {
                _logger.LogError("Connection lost with the remote storage server");
                _uploadOptsCommand.WriteUploadOptsRequested -= uploadOptionsRequested;
            }

            async void uploadOptionsRequested(object sender, WriteUploadOptsRequestedEventArgs e)
            {
                _logger.LogDebug("Sending request to " + e.ServerAddress);

                await responseStream.WriteAsync(new UploadOptsResponse() { UploadOptions= e.UploadOptions.Convert(), ServerAddress = e.ServerAddress });
            }
        }
        public override async Task GetHardwareOptions(IAsyncStreamReader<HardwareOptsResponse> requestStream, IServerStreamWriter<HardwareOptsRequest> responseStream, ServerCallContext context)
        {
            _hardwareOptsCmd.ReadHardwareOptionsRequested += hardwareOptionsRequested;
            try
            {
                await foreach (var message in requestStream.ReadAllAsync())
                {
                    _logger.LogDebug("Request received from " + message.ServerAddress);

                    _hardwareOptsCmd.SetHardwareOptions(message.HardwareOptions.Convert(), message.ServerAddress);
                }

            }
            finally
            {
                _logger.LogError("Connection lost with the remote storage server");
                _hardwareOptsCmd.ReadHardwareOptionsRequested -= hardwareOptionsRequested;
            }

            async void hardwareOptionsRequested(object sender, ReadHardwareOptsRequestedEventArgs e)
            {
                _logger.LogDebug("Sending request to " + e.ServerAddress);

                await responseStream.WriteAsync(new HardwareOptsRequest() { ServerAddress = e.ServerAddress });
            }
        }

        public override async Task SetHardwareOptions(IAsyncStreamReader<HardwareOptsResponse> requestStream, IServerStreamWriter<HardwareOptsResponse> responseStream, ServerCallContext context)
        {
            _hardwareOptsCmd.WriteHardwareOptionsRequested += hardwareOptsRequested;
            try
            {
                await foreach (var message in requestStream.ReadAllAsync())
                {
                    _logger.LogDebug("Request received from " + message.ServerAddress);

                    _hardwareOptsCmd.SetHardwareOptions(message.HardwareOptions.Convert(), message.ServerAddress);
                }
            }
            finally
            {
                _logger.LogError("Connection lost with the remote storage server");
                _hardwareOptsCmd.WriteHardwareOptionsRequested += hardwareOptsRequested;
            }

            async void hardwareOptsRequested(object sender, WriteHardwareOptsRequestedEventArgs e)
            {
                _logger.LogDebug("Sending request to " + e.ServerAddress);

                await responseStream.WriteAsync(new HardwareOptsResponse() { HardwareOptions = e.HardwareOpts.Convert(), ServerAddress = e.ServerAddress });
            }
        }
    }
}
