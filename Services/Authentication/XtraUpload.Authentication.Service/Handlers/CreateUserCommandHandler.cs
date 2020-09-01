using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using XtraUpload.Authentication.Service.Common;
using XtraUpload.Database.Data.Common;
using XtraUpload.Domain;
using XtraUpload.Domain.Infra;
using XtraUpload.WebApp.Common;

namespace XtraUpload.Authentication.Service
{
    /// <summary>
    /// Handles the creation of a new user account
    /// </summary>
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, CreateUserResult>
    {
        readonly IMediator _mediator;
        readonly IUnitOfWork _unitOfWork;
        readonly ILogger<CreateUserCommandHandler> _logger;

        public CreateUserCommandHandler(IUnitOfWork unitOfWork, IMediator mediator, ILogger<CreateUserCommandHandler> logger)
        {
            _logger = logger;
            _mediator = mediator;
            _unitOfWork = unitOfWork;
        }

        public async Task<CreateUserResult> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            CreateUserResult Result = new CreateUserResult();

            // Check if it's not a duplicated user
            User user = await _unitOfWork.Users.FirstOrDefaultAsync(s => s.Email == request.User.Email);
            if (user != null)
            {
                Result.ErrorContent = new ErrorContent("A user with this email already exist", ErrorOrigin.Client);
                return Result;
            }

            // Create the user model
            user = new User()
            {
                UserName = request.User.UserName,
                Email = request.User.Email,
                EmailConfirmed = false,
                Password = Helpers.HashPassword(request.User.Password),
                CreatedAt = DateTime.Now,
                LastModified = DateTime.Now,
                Provider = request.User.Provider,
                Avatar = request.User.Avatar,
                SocialMediaId = request.User.SocialMediaId,
                RoleId = "2", // is the basic user rol see OnModelCreating of ApplicationDbContext
                Theme = Theme.Light
            };

            // Add the new user to the db
            _unitOfWork.Users.Add(user);

            // Save to db
            Result = await _unitOfWork.CompleteAsync(Result);
            if (Result.State != OperationState.Success)
            {
                _logger.LogError("Error occured while creating a new user: " + Result.ErrorContent);
                return Result;
            }
            else
            {
                Result.User = user;
            }

            // Notify listeners about the new user
            await _mediator.Publish(new UserCreatedNotification(user));

            return Result;
        }
    }
}
