using Askmethat.Aspnet.JsonLocalizer.Extensions;
using Askmethat.Aspnet.JsonLocalizer.Localizer;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using XtraUpload.Database.Data.Common;
using Xunit;
using XtraUpload.Services.Test;
using XtraUpload.Authentication.Service.Common;
using System.Linq.Expressions;
using XtraUpload.Domain;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace XtraUpload.Authentication.Service.Test
{
    public class AuthenticationServiceTest
    {
        IMediator _mediator;
        readonly Mock<IUnitOfWork> _mockUnitOfWork;
        readonly Mock<IHttpContextAccessor> _mockHttpContext;

        public AuthenticationServiceTest()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockHttpContext = new Mock<IHttpContextAccessor>();
            _mockHttpContext.Setup(s => s.HttpContext).Returns(new DefaultHttpContext());

            TestServerFactory.Create(app => 
            {
                // system under test `sut`
                _mediator = app.ApplicationServices.GetService<IMediator>();
            },
            configureServices => 
            {
                configureServices.AddJsonLocalization();
                configureServices.AddMediatR(typeof(StandardLoginQueryHandler));
                configureServices.AddScoped<IUnitOfWork>(impl => _mockUnitOfWork.Object);
                configureServices.AddSingleton<IHttpContextAccessor>(impl => _mockHttpContext.Object);
            });
        }
        [Fact]
        async public void CheckPasswordRecoveryInfo_Success()
        {
            // Arrange
            string recoeryId = "recoeryId";
            ConfirmationKey dbKey = new ConfirmationKey()
            {
                GenerateAt = DateTime.UtcNow,
                Status = RequestStatus.InProgress
            };
            _mockUnitOfWork.Setup(unit => unit.ConfirmationKeys.FirstOrDefaultAsync(It.IsAny<Expression<Func<ConfirmationKey, bool>>>()))
                                                .Returns(Task.FromResult(dbKey));
            // Act
            var result = await _mediator.Send(new CheckPwdRecoveryInfoQuery(recoeryId));

            // Assert
            Assert.Equal(OperationState.Success, result.State);
        }

        [Fact]
        async public void CheckPasswordRecoveryInfo_Key_Expired()
        {
            // Arrange
            string recoeryId = "recoeryId";
            ConfirmationKey dbKey = new ConfirmationKey()
            {
                GenerateAt = DateTime.UtcNow.AddDays(-3),
                Status = RequestStatus.InProgress
            };
            _mockUnitOfWork.Setup(unit => unit.ConfirmationKeys.FirstOrDefaultAsync(It.IsAny<Expression<Func<ConfirmationKey, bool>>>()))
                                                .Returns(Task.FromResult(dbKey));
            // Act
            var result = await _mediator.Send(new CheckPwdRecoveryInfoQuery(recoeryId));

            // Assert
            Assert.Equal(OperationState.Failed, result.State);
            Assert.Equal(ErrorOrigin.Client, result.ErrorContent.ErrorType);
            Assert.Equal("The provided token has expired", result.ErrorContent.Message);
        }

        [Fact]
        async public void CheckPasswordRecoveryInfo_Key_Already_Used()
        {
            // Arrange
            string recoeryId = "recoeryId";
            ConfirmationKey dbKey = new ConfirmationKey()
            {
                GenerateAt = DateTime.UtcNow.AddHours(3),
                Status = RequestStatus.Completed
            };
            _mockUnitOfWork.Setup(unit => unit.ConfirmationKeys.FirstOrDefaultAsync(It.IsAny<Expression<Func<ConfirmationKey, bool>>>()))
                                                .Returns(Task.FromResult(dbKey));
            // Act
            var result = await _mediator.Send(new CheckPwdRecoveryInfoQuery(recoeryId));

            // Assert
            Assert.Equal(OperationState.Failed, result.State);
            Assert.Equal(ErrorOrigin.Client, result.ErrorContent.ErrorType);
            Assert.Equal("The provided token has already been used or expired", result.ErrorContent.Message);
        }

        [Fact]
        async public void GetUserByIdQueryHandler_Success()
        {
            // Arrange
            User fakeUser = new User() 
            {
                Id = "fake id",
                UserName = "name",
                Password = "hashed password"
            };
            _mockUnitOfWork.Setup(unit => unit.Users.FirstOrDefaultAsync(It.IsAny<Expression<Func<User, bool>>>()))
                                                .Returns(Task.FromResult(fakeUser));
            // Act
            var result = await _mediator.Send(new GetUserByIdQuery());

            // Assert
            Assert.Equal(OperationState.Success, result.State);
            Assert.Equal(fakeUser.Id, result.User.Id);
            Assert.Equal(fakeUser.UserName, result.User.UserName);
            Assert.Equal(fakeUser.Password, result.User.Password);
        }

        [Fact]
        async public void GetUserByIdQueryHandler_NotFound()
        {
            // Arrange
            _mockUnitOfWork.Setup(unit => unit.Users.FirstOrDefaultAsync(It.IsAny<Expression<Func<User, bool>>>()))
                                                .Returns(Task.FromResult<User>(null));
            // Act
            var result = await _mediator.Send(new GetUserByIdQuery());

            // Assert
            Assert.Equal(OperationState.Failed, result.State);
            Assert.Equal("No user found with the provided id.", result.ErrorContent.Message);
        }
    }
}
