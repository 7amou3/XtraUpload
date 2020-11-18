using MediatR;
using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using XtraUpload.Administration.Service.Common;
using XtraUpload.Database.Data.Common;
using XtraUpload.Domain;
using XtraUpload.Domain.Infra;

namespace XtraUpload.Administration.Service
{
    /// <summary>
    /// Add a new page
    /// </summary>
    public class AddPageCommandHandler : IRequestHandler<AddPageCommand, PageResult>
    {
        readonly IUnitOfWork _unitOfWork;
        public AddPageCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<PageResult> Handle(AddPageCommand request, CancellationToken cancellationToken)
        {
            PageResult result = new PageResult();
            // Check page name is unique
            Page pageNameUnique = await _unitOfWork.Pages.FirstOrDefaultAsync(s => s.Name == request.Page.Name);
            if (pageNameUnique != null)
            {
                result.ErrorContent = new ErrorContent($"A page with the same name already exists", ErrorOrigin.Client);
                return result;
            }
            request.Page.Id = Helpers.GenerateUniqueId();
            request.Page.CreatedAt = DateTime.Now;
            request.Page.UpdatedAt = DateTime.Now;
            request.Page.Url = Regex.Replace(request.Page.Name.ToLower(), @"\s+", "_");
            await _unitOfWork.Pages.AddAsync(request.Page);

            // Save to db
            result = await _unitOfWork.CompleteAsync(result);
            if (result.State == OperationState.Success)
            {
                result.Page = request.Page;
            }

            return result;
        }
    }
}
