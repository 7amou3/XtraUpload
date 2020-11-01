using MediatR;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XtraUpload.Administration.Service.Common;
using XtraUpload.Database.Data.Common;
using XtraUpload.Domain;
using System.Linq;

namespace XtraUpload.Administration.Service
{
    public class DeleteStorageServerCommandHandler : IRequestHandler<DeleteStorageServerCommand, StorageServerResult>
    {
        readonly IUnitOfWork _unitOfWork;
        public DeleteStorageServerCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<StorageServerResult> Handle(DeleteStorageServerCommand request, CancellationToken cancellationToken)
        {
            StorageServerResult Result = new StorageServerResult();
            if (!Guid.TryParse(request.ServerId, out Guid serverId))
            {
                Result.ErrorContent = new ErrorContent("The provided id is not valid.", ErrorOrigin.Client);
                return Result;
            }
            // Get the server from db
            StorageServer server = await _unitOfWork.StorageServer.FirstOrDefaultAsync(s => s.Id == serverId);
            if (server == null)
            {
                Result.ErrorContent = new ErrorContent("The server with the provided id was not found.", ErrorOrigin.Client);
                return Result;
            }

            // remove from collection
            _unitOfWork.StorageServer.Remove(server);
            // remove files
            IEnumerable<FileItem> files = await _unitOfWork.Files.FindAsync(s => s.StorageServerId == serverId);
            if (files.Any())
            {
                _unitOfWork.Files.RemoveRange(files);
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
