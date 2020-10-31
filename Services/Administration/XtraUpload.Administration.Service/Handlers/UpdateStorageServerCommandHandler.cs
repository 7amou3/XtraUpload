using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XtraUpload.Administration.Service.Common;
using XtraUpload.Database.Data.Common;
using XtraUpload.Domain;
using XtraUpload.GrpcServices.Common;

namespace XtraUpload.Administration.Service
{
    public class UpdateStorageServerCommandHandler : IRequestHandler<UpdateStorageServerCommand, StorageServerResult>
    {
        readonly IUnitOfWork _unitOfWork;
        readonly IHardwareOptsClientProxy _hardwareOptsProxy;
        readonly IUploadOptsClientProxy _uploadOptsClientProxy;
        readonly ILogger<AddStorageServerCommandHandler> _logger;

        public UpdateStorageServerCommandHandler(
            IUnitOfWork unitOfWork,
            IHardwareOptsClientProxy hardwareOptsProxy,
            IUploadOptsClientProxy uploadOptsClientProxy,
            ILogger<AddStorageServerCommandHandler> logger
            )
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _hardwareOptsProxy = hardwareOptsProxy;
            _uploadOptsClientProxy = uploadOptsClientProxy;
        }

        public async Task<StorageServerResult> Handle(UpdateStorageServerCommand request, CancellationToken cancellationToken)
        {
            StorageServerResult Result = new StorageServerResult();
            // Check guid is valid
            if(! Guid.TryParse(request.Id, out Guid serverId))
            {
                Result.ErrorContent = new ErrorContent("Invalid server Id.", ErrorOrigin.Client);
                return Result;
            }
            // Get the server from db
            StorageServer server = await _unitOfWork.StorageServer.FirstOrDefaultAsync(s => s.Id == serverId);
            if (server == null)
            {
                Result.ErrorContent = new ErrorContent("The server with the provided id was not found.", ErrorOrigin.Client);
                return Result;
            }
            // update the server
            server.Address = request.StorageInfo.Address;
            server.State = request.StorageInfo.State;

            // Update upload options config
            var writeUploadOptsResult = await _uploadOptsClientProxy.WriteUploadOptions(request.UploadOpts, request.StorageInfo.Address);
            if (writeUploadOptsResult.State != OperationState.Success)
            {
                Result.ErrorContent = new ErrorContent($"Error occured while writing {nameof(UploadOptions)} to server {request.StorageInfo.Address}", ErrorOrigin.Server);
                return Result;
            }
            // Update hardware config
            var writeHardwareOptsResult = await _hardwareOptsProxy.WriteHardwareOptions(request.HardwareOpts, request.StorageInfo.Address);
            if (writeHardwareOptsResult.State != OperationState.Success)
            {
                Result.ErrorContent = new ErrorContent($"Error occured while writing {nameof(HardwareCheckOptions)} to server {request.StorageInfo.Address}", ErrorOrigin.Server);
                return Result;
            }

            // everything ok, update the server
            Result = await _unitOfWork.CompleteAsync(Result);
            if (Result.State != OperationState.Success)
            {
                Result.ErrorContent = new ErrorContent("An error occured while saving the record to the database. " + Result.ErrorContent.Message, ErrorOrigin.Server);
                return Result;
            }
            Result.Server = server;
            return Result;
        }
    }
}
