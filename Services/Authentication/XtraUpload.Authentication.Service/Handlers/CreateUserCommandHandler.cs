using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using XtraUpload.Authentication.Service.Common;
using XtraUpload.Database.Data.Common;
using XtraUpload.Domain;

namespace XtraUpload.Authentication.Service
{
    /// <summary>
    /// Handles the creation of a new user account
    /// </summary>
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, CreateUserResult>
    {
        #region Fields
        readonly IMediator _mediator;
        readonly IUnitOfWork _unitOfWork;
        readonly ILogger<CreateUserCommandHandler> _logger;
        #endregion

        #region Constructor
        public CreateUserCommandHandler(IUnitOfWork unitOfWork, IMediator mediator, ILogger<CreateUserCommandHandler> logger)
        {
            _logger = logger;
            _mediator = mediator;
            _unitOfWork = unitOfWork;
        }
        #endregion

        #region Handler
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

            IEnumerable<Language> languages = await _unitOfWork.Languages.GetAll();
            string[] culture = request.Language.Split('-'); // en-US
            var validLang = languages.FirstOrDefault(s => s.Culture == culture[0]);

            // Create the user model
            user = new User()
            {
                UserName = request.User.UserName,
                Email = request.User.Email,
                EmailConfirmed = false,
                Password = Helpers.HashPassword(request.User.Password),
                CreatedAt = DateTime.UtcNow,
                LastModified = DateTime.UtcNow,
                Provider = request.User.Provider,
                Avatar = request.User.Avatar,
                SocialMediaId = request.User.SocialMediaId,
                RoleId = "2", // is the basic user rol see OnModelCreating of ApplicationDbContext
                Theme = Theme.Light,
                LanguageId = validLang == null ? languages.First(s => s.Default).Id : validLang.Id
            };

            // Add the new user to the db
            await _unitOfWork.Users.AddAsync(user);

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
        #endregion
    }
}
