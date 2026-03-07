using Absence.Application.Identity;
using Absence.Application.UseCases.Users.Commands;
using Absence.Application.UseCases.Users.Handlers;
using Absence.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using NSubstitute;
using Shouldly;

namespace Absence.Application.Tests.UseCases.Users;

public class AddUserHandlerTests
{
    private readonly AddUserCommand _command = new(
        new() 
        { 
            Credentials = new() { Email = "email", Password = "password" } 
        }
    );
    private readonly IUserService _userService;
    private readonly AddUserHandler _sut;

    public AddUserHandlerTests()
    {
        _userService = Substitute.For<IUserService>();
        _sut = new AddUserHandler(_userService);
    }

    [Fact]
    public async Task Handle_ReturnsError_WhenCreatingFailed()
    {
        //Arrange
        _userService.CreateAsync(
            Arg.Is<UserEntity>(_ => 
                _.FirstName == _command.User.FirstName
                && _.LastName == _command.User.LastName
                && _.Email == _command.User.Credentials.Email
                && _.UserName == _command.User.Credentials.Email
            ), 
            _command.User.Credentials.Password
        ).Returns(IdentityResult.Failed());

        //Act
        var actual = await _sut.Handle(_command);

        //Assert
        actual.IsT1.ShouldBeTrue();
    }

    [Fact]
    public async Task Handle_ReturnsSuccess_WhenUserCreated()
    {
        //Arrange
        _userService.CreateAsync(
            Arg.Is<UserEntity>(_ =>
                _.FirstName == _command.User.FirstName
                && _.LastName == _command.User.LastName
                && _.Email == _command.User.Credentials.Email
                && _.UserName == _command.User.Credentials.Email
            ),
            _command.User.Credentials.Password
        ).Returns(IdentityResult.Success);

        //Act
        var actual = await _sut.Handle(_command);

        //Assert
        actual.IsT0.ShouldBeTrue();
    }
}