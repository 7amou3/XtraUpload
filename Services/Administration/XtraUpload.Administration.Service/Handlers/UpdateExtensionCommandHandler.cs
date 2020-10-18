using MediatR;
using System.Threading;
using System.Threading.Tasks;
using XtraUpload.Administration.Service.Common;
using XtraUpload.Database.Data.Common;
using XtraUpload.Domain;

namespace XtraUpload.Administration.Service
{
    public class UpdateExtensionCommandHandler : IRequestHandler<UpdateExtensionCommand, FileExtensionResult>
    {
        readonly IUnitOfWork _unitOfWork;
        public UpdateExtensionCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<FileExtensionResult> Handle(UpdateExtensionCommand request, CancellationToken cancellationToken)
        {
            FileExtensionResult result = new FileExtensionResult();
            FileExtension ext = await _unitOfWork.FileExtensions.FirstOrDefaultAsync(s => s.Id == request.Id);
            if (ext == null)
            {
                result.ErrorContent = new ErrorContent("No file type found with the provided id.", ErrorOrigin.Client);
                return result;
            }

            // update extension name
            ext.Name = request.Name;

            // Save to db
            return await _unitOfWork.CompleteAsync(result);
        }
    }
}
