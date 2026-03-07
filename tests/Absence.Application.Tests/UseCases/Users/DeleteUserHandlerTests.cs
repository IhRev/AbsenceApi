using Absence.Application.Common.Interfaces;
using Absence.Application.Identity;
using Absence.Application.UseCases.Users.Commands;
using Absence.Application.UseCases.Users.Handlers;
using Absence.Domain.Entities;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Shouldly;

namespace Absence.Application.Tests.UseCases.Users;

public class DeleteUserHandlerTests
{
    private const string USER_ID = "id";
    private IUserService _userService;
    private IUser _user;
    private DeleteUserHandler _sut;

    public DeleteUserHandlerTests()
    {
        _userService = Substitute.For<IUserService>();
        _user = Substitute.For<IUser>();
        _sut = new DeleteUserHandler(_userService, _user);
    }

    [Fact]
    public async Task Handle_ReturnsSuccess_WhenDeletedSuccessfully()
    {
        //Arrange
        var request = new DeleteUserCommand(new() { Password = "password" });

         _user.Id.Returns(USER_ID);

        var user = new UserEntity
        {
            FirstName = "first",
            LastName = "last"
        };
        _userService.FindByIdAsync(USER_ID).Returns(user);

        _userService.CheckPasswordAsync(user, request.Request.Password).Returns(true);

        //Act
        var actual = await _sut.Handle(request);

        //Assert
        actual.IsT0.ShouldBeTrue();
        await _userService.Received().DeleteAsync(user);
    }

    [Fact]
    public async Task Handle_ReturnsBadRequest_WhenPasswordIncorrect()
    {
        //Arrange
        var request = new DeleteUserCommand(new() { Password = "password" });

        _user.Id.Returns(USER_ID);

        var user = new UserEntity
        {
            FirstName = "first",
            LastName = "last"
        };
        _userService.FindByIdAsync(USER_ID).Returns(user);

        _userService.CheckPasswordAsync(user, request.Request.Password).Returns(false);

        //Act
        var actual = await _sut.Handle(request);

        //Assert
        actual.IsT1.ShouldBeTrue();
        await _userService.DidNotReceive().DeleteAsync(user);
    }

    [Fact]
    public async Task Handle_ReturnsNotFound_WhenUserDoesntExist()
    {
        //Arrange
        var request = new DeleteUserCommand(new() { Password = "password" });

        _user.Id.Returns(USER_ID);

        _userService.FindByIdAsync(USER_ID).ReturnsNull();

        //Act
        var actual = await _sut.Handle(request);

        //Assert
        actual.IsT2.ShouldBeTrue();
        await _userService.DidNotReceive().DeleteAsync(Arg.Any<UserEntity>());
    }
}