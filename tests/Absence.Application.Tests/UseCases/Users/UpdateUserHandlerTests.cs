using Absence.Application.Common.Interfaces;
using Absence.Application.Identity;
using Absence.Application.UseCases.Users.Commands;
using Absence.Application.UseCases.Users.Handlers;
using Absence.Domain.Entities;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Shouldly;

namespace Absence.Application.Tests.UseCases.Users;

public class UpdateUserHandlerTests
{
    private const string USER_ID = "id";
    private IUserService _userService;
    private IUser _user;
    private UpdateUserHandler _sut;

    public UpdateUserHandlerTests()
    {
        _userService = Substitute.For<IUserService>();
        _user = Substitute.For<IUser>();
        _sut = new(_userService, _user);
    }

    [Fact]
    public async Task Handle_ReturnsSuccess_WhenUpdatedSuccessfully()
    {
        //Arrange
        var request = new UpdateUserCommand(
            new() 
            { 
                Email = "email",
                FirstName = "name",
                LastName = "last"
            }
        );

        _user.Id.Returns(USER_ID);

        var user = new UserEntity
        {
            FirstName = "first",
            LastName = "last"
        };
        _userService.FindByIdAsync(USER_ID).Returns(user);

        //Act
        var actual = await _sut.Handle(request);

        //Assert
        actual.IsT0.ShouldBeTrue();
        await _userService.Received().UpdateAsync(user);
    }

    [Fact]
    public async Task Handle_ReturnsNotFound_WhenUserDoesntExist()
    {
        //Arrange
        var request = new UpdateUserCommand(
            new()
            {
                Email = "email",
                FirstName = "name",
                LastName = "last"
            }
        );

        _user.Id.Returns(USER_ID);

        _userService.FindByIdAsync(USER_ID).ReturnsNull();

        //Act
        var actual = await _sut.Handle(request);

        //Assert
        actual.IsT1.ShouldBeTrue();
        await _userService.DidNotReceive().UpdateAsync(Arg.Any<UserEntity>());
    }
}