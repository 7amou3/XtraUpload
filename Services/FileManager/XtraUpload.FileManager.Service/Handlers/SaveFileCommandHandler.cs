using MediatR;
using Microsoft.EntityFrameworkCore.Internal;
using System.Linq;
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
            CreateFileResult Result = new CreateFileResult();

            // Add file to collection
            await _unitOfWork.Files.AddAsync(request.File);
            // Save to db
            Result = await _unitOfWork.CompleteAsync(Result);
            var files = await _unitOfWork.Files.GetFilesServerInfo(s => s.Id == request.File.Id);
            if (!files.Any())
            {
                Result.ErrorContent = new ErrorContent("No file found with the provided id", ErrorOrigin.Server);
                return Result;
            }
            if (Result.State == OperationState.Success)
            {
                Result.File = files.ElementAt(0);
            }

            return Result;
        }
    }
}
