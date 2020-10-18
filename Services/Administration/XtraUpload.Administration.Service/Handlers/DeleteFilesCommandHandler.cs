using MediatR;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XtraUpload.Administration.Service.Common;
using XtraUpload.Database.Data.Common;
using XtraUpload.Domain;

namespace XtraUpload.Administration.Service
{
    /// <summary>
    /// Delete file(s) by id
    /// </summary>
    public class DeleteFilesCommandHandler : IRequestHandler<DeleteFilesCommand, DeleteFilesResult>
    {
        readonly IUnitOfWork _unitOfWork;

        public DeleteFilesCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<DeleteFilesResult> Handle(DeleteFilesCommand request, CancellationToken cancellationToken)
        {
            DeleteFilesResult result = new DeleteFilesResult();
            IEnumerable<FileItem> files = await _unitOfWork.Files.FindAsync(s => request.FilesId.Contains(s.Id));
            if (files.Any())
            {
                // Mark files for deletion
                foreach (var file in files)
                {
                    file.Status = ItemStatus.To_Be_Deleted;
                }

                // Save to db
                result = await _unitOfWork.CompleteAsync(result);
            }
            else
            {
                result.ErrorContent = new ErrorContent($"Please check the provided id(s)", ErrorOrigin.Server);
            }

            result.Files = files;
            return result;
        }
    }
}
