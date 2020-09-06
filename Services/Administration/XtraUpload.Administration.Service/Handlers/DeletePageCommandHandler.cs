using MediatR;
using System.Threading;
using System.Threading.Tasks;
using XtraUpload.Administration.Service.Common;
using XtraUpload.Database.Data.Common;
using XtraUpload.Domain;

namespace XtraUpload.Administration.Service.Handlers
{
    /// <summary>
    /// Delete a Page
    /// </summary>
    public class DeletePageCommandHandler : IRequestHandler<DeletePageCommand, OperationResult>
    {
        readonly IUnitOfWork _unitOfWork;

        public DeletePageCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<OperationResult> Handle(DeletePageCommand request, CancellationToken cancellationToken)
        {
            OperationResult result = new OperationResult();
            // Get page name
            Page page = await _unitOfWork.Pages.FirstOrDefaultAsync(s => s.Id == request.PageId);
            if (page == null)
            {
                result.ErrorContent = new ErrorContent($"The requested page was not found", ErrorOrigin.Client);
                return result;
            }

            _unitOfWork.Pages.Remove(page);

            // Save to db
            return await _unitOfWork.CompleteAsync(result);
        }
    }
}
