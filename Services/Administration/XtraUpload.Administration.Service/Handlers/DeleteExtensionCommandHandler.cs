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
    /// <summary>
    /// Delete extension
    /// </summary>
    public class DeleteExtensionCommandHandler : IRequestHandler<DeleteExtensionCommand, OperationResult>
    {
        readonly IUnitOfWork _unitOfWork;
        public DeleteExtensionCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<OperationResult> Handle(DeleteExtensionCommand request, CancellationToken cancellationToken)
        {
            OperationResult result = new OperationResult();
            FileExtension ext = await _unitOfWork.FileExtensions.FirstOrDefaultAsync(s => s.Id == request.ExtensionId);
            if (ext == null)
            {
                result.ErrorContent = new ErrorContent("No file type found with the provided id.", ErrorOrigin.Client);
                return result;
            }

            // remove from collection
            _unitOfWork.FileExtensions.Remove(ext);

            // Save to db
            return await _unitOfWork.CompleteAsync(result);
        }
    }
}
