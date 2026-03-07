using Absence.Application.Common.Interfaces;
using Absence.Application.Identity;
using Absence.Application.UseCases.Users.Commands;
using Absence.Application.UseCases.Users.Handlers;
using Absence.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Shouldly;

namespace Absence.Application.Tests.UseCases.Users;

public class ChangePasswordHandlerTests
{
    private readonly string _description = "description";
    private readonly string _userId = "userId";
    private readonly IUserService _userService;
    private readonly IUser _user;
    private ChangePasswordHandler _sut;

    public ChangePasswordHandlerTests()
    {
        _userService = Substitute.For<IUserService>();
        _user = Substitute.For<IUser>();
        _sut = new ChangePasswordHandler(_userService, _user);
    }

    [Fact]
    public async Task Handle_ReturnsNotFound_WhenUserNotFound()
    {
        //Arrange
        var request = new ChangePasswordCommand(
            new()
            {
                OldPassword = "old",
                NewPassword = "new"
            }
        );

        _user.Id.Returns(_userId);

        _userService.FindByIdAsync(_userId).ReturnsNull();

        //Act
        var actual = await _sut.Handle(request);

        //Assert
        actual.IsT2.ShouldBeTrue();
        await _userService
            .DidNotReceive()
            .ChangePasswordAsync(Arg.Any<UserEntity>(), request.Request.OldPassword, request.Request.NewPassword);
    }

    [Fact]
    public async Task Handle_ReturnsBadRequests_WhenChangePasswordFailed()
    {
        //Arrange
        var request = new ChangePasswordCommand(
            new()
            {
                OldPassword = "old",
                NewPassword = "new"
            }
        );

        _user.Id.Returns(_userId);

        var user = new UserEntity()
        {
            FirstName = "first",
            LastName = "last"
        };
        _userService.FindByIdAsync(_userId).Returns(user);

        _userService
            .ChangePasswordAsync(user, request.Request.OldPassword, request.Request.NewPassword)
            .Returns(IdentityResult.Failed(new IdentityError() { Description = _description }));

        //Act
        var actual = await _sut.Handle(request);

        //Assert
        actual.IsT1.ShouldBeTrue();
        actual.AsT1.Message.ShouldBe(_description);
        await _userService
            .DidNotReceive()
            .UpdateAsync(user);
    }

    [Fact]
    public async Task Handle_ReturnsSuccess_WhenPasswordChanged()
    {
        //Arrange
        var request = new ChangePasswordCommand(
            new()
            {
                OldPassword = "old",
                NewPassword = "new"
            }
        );

        _user.Id.Returns(_userId);

        var user = new UserEntity()
        {
            FirstName = "first",
            LastName = "last",
            RefreshToken = "token"
        };
        _userService.FindByIdAsync(_userId).Returns(user);

        _userService
            .ChangePasswordAsync(user, request.Request.OldPassword, request.Request.NewPassword)
            .Returns(IdentityResult.Success);

        //Act
        var actual = await _sut.Handle(request);

        //Assert
        actual.IsT0.ShouldBeTrue();
        user.RefreshToken.ShouldBeNull();
        user.RefreshTokenExpiresAt.ShouldBe(DateTimeOffset.MinValue);
        await _userService
            .Received()
            .UpdateAsync(user);
    }
}