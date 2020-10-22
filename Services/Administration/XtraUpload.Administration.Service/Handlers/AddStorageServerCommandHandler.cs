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
    public class AddStorageServerCommandHandler : IRequestHandler<AddStorageServerCommand, StorageServerResult>
    {
        readonly IUnitOfWork _unitOfWork;
        readonly IHardwareOptsClientProxy _hardwareOptsProxy;
        readonly IUploadOptsClientProxy _uploadOptsClientProxy;
        readonly ILogger<AddStorageServerCommandHandler> _logger;

        public AddStorageServerCommandHandler(
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
        public async Task<StorageServerResult> Handle(AddStorageServerCommand request, CancellationToken cancellationToken)
        {
            StorageServerResult Result = new StorageServerResult();
            StorageServer server = await _unitOfWork.StorageServer.FirstOrDefaultAsync(s => s.Address == request.StorageInfo.Address);
            if (server != null)
            {
                Result.ErrorContent = new ErrorContent("A server with the same address already exists.", ErrorOrigin.Client);
                return Result;
            }
            // Add the server to db
            StorageServer newServer = new StorageServer() 
            {
                Address = request.StorageInfo.Address,
                State = request.StorageInfo.State 
            };
            _unitOfWork.StorageServer.Add(newServer);

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

            // everything ok, save the new server to db
            Result = await _unitOfWork.CompleteAsync(Result);
            if (Result.State != OperationState.Success)
            {
                Result.ErrorContent = new ErrorContent("An error occured while saving the record to the database. " + Result.ErrorContent.Message, ErrorOrigin.Server);
                return Result;
            }
            Result.Server = newServer;
            return Result;

        }
    }
}
