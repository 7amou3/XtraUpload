using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using XtraUpload.Administration.Service.Common;
using XtraUpload.Database.Data.Common;
using XtraUpload.Domain;

namespace XtraUpload.Administration.Service
{
    /// <summary>
    /// Update a page
    /// </summary>
    public class UpdatePageCommandHandler : IRequestHandler<UpdatePageCommand, PageResult>
    {
        readonly IUnitOfWork _unitOfWork;
        public UpdatePageCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<PageResult> Handle(UpdatePageCommand request, CancellationToken cancellationToken)
        {
            PageResult result = new PageResult();
            Page page = await _unitOfWork.Pages.FirstOrDefaultAsync(s => s.Id == request.Page.Id);
            // Check page exist
            if (page == null)
            {
                result.ErrorContent = new ErrorContent($"The page {request.Page.Name} does not exist", ErrorOrigin.Client);
                return result;
            }
            // Check page name is unique
            Page pageNameUnique = await _unitOfWork.Pages.FirstOrDefaultAsync(s => s.Name == request.Page.Name && s.Id != request.Page.Id);
            if (pageNameUnique != null)
            {
                result.ErrorContent = new ErrorContent($"A page with the same name already exists", ErrorOrigin.Client);
                return result;
            }
            // Update
            page.UpdatedAt = DateTime.Now;
            page.Content = request.Page.Content;
            page.Name = request.Page.Name;
            page.Url = Regex.Replace(request.Page.Name.ToLower(), @"\s+", "_");

            // Save to db
            result = await _unitOfWork.CompleteAsync(result);
            if (result.State == OperationState.Success)
            {
                result.Page = page;
            }
            return result;
        }
    }
}
