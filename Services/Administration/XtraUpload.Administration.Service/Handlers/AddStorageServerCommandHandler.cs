using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XtraUpload.Administration.Service.Common;
using XtraUpload.Database.Data.Common;
using XtraUpload.Domain;

namespace XtraUpload.Administration.Service
{
    public class AddStorageServerCommandHandler : IRequestHandler<AddStorageServerCommand, StorageServerResult>
    {
        IUnitOfWork _unitOfWork;
        public AddStorageServerCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<StorageServerResult> Handle(AddStorageServerCommand request, CancellationToken cancellationToken)
        {
            StorageServerResult Result = new StorageServerResult();
            //StorageServer server = await _unitOfWork.StorageServer.FirstOrDefaultAsync(s => s.Address == request.StorageInfo.Address);
            //if (server != null)
            //{
            //    Result.ErrorContent = new ErrorContent("A server with the same address already exists.", ErrorOrigin.Client);
            //    return Result;
            //}
            // Add the server to db
            StorageServer newServer = new StorageServer() 
            {
                Address = request.StorageInfo.Address,
                State = request.StorageInfo.State 
            };
            _unitOfWork.StorageServer.Add(newServer);
            // Save
            Result = await _unitOfWork.CompleteAsync(Result);
            if (Result.State == OperationState.Success)
            {
                Result.Server = newServer;
            }
            // todo: Update server config
            return Result;

        }
    }
}
