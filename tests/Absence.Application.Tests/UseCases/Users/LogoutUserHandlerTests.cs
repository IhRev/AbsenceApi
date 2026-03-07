using Absence.Application.Common.Interfaces;
using Absence.Application.Identity;
using Absence.Application.UseCases.Users.Commands;
using Absence.Application.UseCases.Users.Handlers;
using Absence.Domain.Entities;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Shouldly;

namespace Absence.Application.Tests.UseCases.Users;

public class LogoutUserHandlerTests
{
    private const string USER_ID = "id";
    private readonly IUserService _userService;
    private readonly IUser _user;
    private readonly LogoutUserHandler _sut;

    public LogoutUserHandlerTests()
    {
        _userService = Substitute.For<IUserService>();
        _user = Substitute.For<IUser>();
        _sut = new LogoutUserHandler(_userService, _user);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenLoggedOutSuccessfully()
    {
        //Arrange
        var request = new LogoutUserCommand();

        _user.Id.Returns(USER_ID);

        var user = new UserEntity
        {
            FirstName = "first",
            LastName = "last",
            RefreshToken = "token",
            RefreshTokenExpiresAt = DateTimeOffset.UtcNow
        };
        _userService.FindByIdAsync(USER_ID).Returns(user);

        //Act
        var actual = await _sut.Handle(request);

        //Assert
        actual.IsT0.ShouldBeTrue();
        user.RefreshToken.ShouldBeNull();
        user.RefreshTokenExpiresAt.ShouldBe(DateTimeOffset.MinValue);
        await _userService.Received().UpdateAsync(user);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenUserDoesntExist()
    {
        //Arrange
        var request = new LogoutUserCommand();

        _user.Id.Returns(USER_ID);

        _userService.FindByIdAsync(USER_ID).ReturnsNull();

        //Act
        var actual = await _sut.Handle(request);

        //Assert
        actual.IsT1.ShouldBeTrue();
        await _userService.DidNotReceive().UpdateAsync(Arg.Any<UserEntity>());
    }
}