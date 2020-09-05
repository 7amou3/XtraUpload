using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using XtraUpload.Database.Data.Common;
using XtraUpload.Domain;
using XtraUpload.Domain.Infra;
using XtraUpload.Email.Service.Common;
using XtraUpload.Setting.Service.Common;

namespace XtraUpload.Setting.Service
{
    /// <summary>
    /// Process the command when a user request to confirm his email
    /// </summary>
    public class RequestConfirmationEmailCommandHandler : IRequestHandler<RequestConfirmationEmailCommand, OperationResult>
    {
        readonly IMediator _mediatr;
        readonly ClaimsPrincipal _caller;
        readonly IUnitOfWork _unitOfWork;
        
        public RequestConfirmationEmailCommandHandler(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, IMediator mediatr)
        {
            _mediatr = mediatr;
            _unitOfWork = unitOfWork;
            _caller = httpContextAccessor.HttpContext.User;
        }
        
        public async Task<OperationResult> Handle(RequestConfirmationEmailCommand request, CancellationToken cancellationToken)
        {
            string userId = _caller.GetUserId();
            OperationResult Result = new OperationResult();
            User user = await _unitOfWork.Users.FirstOrDefaultAsync(s => s.Id == userId);
            // Check user exist
            if (user == null)
            {
                Result.ErrorContent = new ErrorContent("No user found with the provided email.", ErrorOrigin.Client);
                return Result;
            }

            // Check email service is up
            HealthCheckResult health = await _mediatr.Send(new EmailHealthCheckQuery());
            if (health.Status != HealthStatus.Healthy)
            {
                Result.ErrorContent = new ErrorContent("Internal email server error, please check again later.", ErrorOrigin.Server);
                return Result;
            }
            // Check email confirmation status
            if (user.EmailConfirmed)
            {
                Result.ErrorContent = new ErrorContent("The email has been already confirmed.", ErrorOrigin.Client);
                return Result;
            }
            // Create and store a mail confirmation token
            ConfirmationKey token = new ConfirmationKey()
            {
                Id = Helpers.GenerateUniqueId(),
                GenerateAt = DateTime.Now,
                Status = RequestStatus.InProgress,
                UserId = userId,
                IpAdress = request.UserIp
            };
            _unitOfWork.ConfirmationKeys.Add(token);
            // Save changes to db
            Result = await _unitOfWork.CompleteAsync(Result);
            if (Result.State == OperationState.Success)
            {
                // Send confirmation email 
                await _mediatr.Send(new SendConfirmationEmailCommand(user, token));
            }

            return Result;
        }

    }
}
