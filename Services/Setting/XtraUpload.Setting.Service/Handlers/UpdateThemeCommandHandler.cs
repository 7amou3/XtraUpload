using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using XtraUpload.Database.Data.Common;
using XtraUpload.Domain;
using XtraUpload.Domain.Infra;
using XtraUpload.Setting.Service.Common;

namespace XtraUpload.Setting.Service
{
    /// <summary>
    /// Update the user theme
    /// </summary>
    public class UpdateThemeCommandHandler : IRequestHandler<UpdateThemeCommand, OperationResult>
    {
        readonly ClaimsPrincipal _caller;
        readonly IUnitOfWork _unitOfWork;
       
        public UpdateThemeCommandHandler(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _caller = httpContextAccessor.HttpContext.User;
        }
        
        public async Task<OperationResult> Handle(UpdateThemeCommand request, CancellationToken cancellationToken)
        {
            string userId = _caller.GetUserId();
            OperationResult result = new OperationResult();
            User user = await _unitOfWork.Users.FirstOrDefaultAsync(s => s.Id == userId);
            // Check user exist
            if (user == null)
            {
                result.ErrorContent = new ErrorContent("No user found with the provided email.", ErrorOrigin.Client);
                return result;
            }
            // Update
            user.Theme = request.Theme;
            user.LastModified = DateTime.Now;

            // Save to db
            return await _unitOfWork.CompleteAsync(result);
        }
    }
}
