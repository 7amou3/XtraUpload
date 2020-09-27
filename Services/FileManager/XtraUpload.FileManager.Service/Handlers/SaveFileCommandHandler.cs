using MediatR;
using System.Threading;
using System.Threading.Tasks;
using XtraUpload.Database.Data.Common;
using XtraUpload.Domain;
using XtraUpload.FileManager.Service.Common;

namespace XtraUpload.FileManager.Service
{
    /// <summary>
    /// Save the file to db
    /// </summary>
    public class SaveFileCommandHandler : IRequestHandler<SaveFileCommand, CreateFileResult>
    {
        readonly IUnitOfWork _unitOfWork;

        public SaveFileCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CreateFileResult> Handle(SaveFileCommand request, CancellationToken cancellationToken)
        {
            CreateFileResult result = new CreateFileResult();

            // Add file to collection
            _unitOfWork.Files.Add(request.File);
            // Save to db
            result = await _unitOfWork.CompleteAsync(result);
            if (result.State == OperationState.Success)
            {
                result.File = request.File;
            }

            return result;
        }
    }
}
