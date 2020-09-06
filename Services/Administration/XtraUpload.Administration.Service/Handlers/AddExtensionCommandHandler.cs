using MediatR;
using System.Threading;
using System.Threading.Tasks;
using XtraUpload.Administration.Service.Common;
using XtraUpload.Database.Data.Common;
using XtraUpload.Domain;

namespace XtraUpload.Administration.Service
{
    /// <summary>
    /// Add an extension
    /// </summary>
    public class AddExtensionCommandHandler : IRequestHandler<AddExtensionCommand, FileExtensionResult>
    {
        readonly IUnitOfWork _unitOfWork;
        public AddExtensionCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<FileExtensionResult> Handle(AddExtensionCommand request, CancellationToken cancellationToken)
        {
            FileExtensionResult result = new FileExtensionResult();
            FileExtension newFileType = new FileExtension()
            {
                Name = request.ExtName
            };
            _unitOfWork.FileExtensions.Add(newFileType);

            // Save to db
            result = await _unitOfWork.CompleteAsync(result);
            if (result.State == OperationState.Success)
            {
                result.FileExtension = newFileType;
            }

            return result;
        }
    }
}
