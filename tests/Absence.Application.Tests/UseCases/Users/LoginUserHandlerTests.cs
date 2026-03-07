using Absence.Application.Common.Interfaces;
using Absence.Application.Identity;
using Absence.Application.UseCases.Users.Commands;
using Absence.Application.UseCases.Users.Handlers;
using Absence.Domain.Entities;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Shouldly;

namespace Absence.Application.Tests.UseCases.Users;

public class LoginUserHandlerTests
{
    private readonly LoginUserCommand _request = new(new() { Email = "email", Password = "password" });
    private readonly IUserService _userService;
    private readonly IJwtService _jwtService;
    private readonly IRefreshTokenService _refreshTokenService;
    private LoginUserHandler _sut;

    public LoginUserHandlerTests()
    {
        _userService = Substitute.For<IUserService>();
        _jwtService = Substitute.For<IJwtService>();
        _refreshTokenService = Substitute.For<IRefreshTokenService>();
        _sut = new LoginUserHandler(_userService, _jwtService, _refreshTokenService);
    }

    [Fact]
    public async Task Handle_ReturnsFail_WhenUserNotFound()
    {
        //Arrange
        _userService
            .FindByEmailAsync(_request.Credentials.Email)
            .ReturnsNull();

        //Act
        var actual = await _sut.Handle(_request);

        //Assert
        actual.IsSuccess.ShouldBeFalse();
    }

    [Fact]
    public async Task Handle_ReturnsFail_WhenPasswordIsIncorrect()
    {
        //Arrange
        var user = new UserEntity
        {
            FirstName = "FirstName",
            LastName = "LastName",
        };
        _userService
            .FindByEmailAsync(_request.Credentials.Email)
            .Returns(user);

        _userService
            .CheckPasswordAsync(user, _request.Credentials.Password)
            .Returns(false);

        //Act
        var actual = await _sut.Handle(_request);

        //Assert
        actual.IsSuccess.ShouldBeFalse();
    }

    [Fact]
    public async Task Handle_ReturnsSuccessResult_WhenLoggedInSuccessfully()
    {
        //Arrange
        var user = new UserEntity
        {
            FirstName = "FirstName",
            LastName = "LastName",
        };
        _userService
            .FindByEmailAsync(_request.Credentials.Email)
            .Returns(user);

        _userService
            .CheckPasswordAsync(user, _request.Credentials.Password)
            .Returns(true);

        var accessToken = "accessToken";
        _jwtService
            .GenerateToken(user)
            .Returns(accessToken);

        var refreshToken = "refreshToken";
        _refreshTokenService
            .GenerateToken(user)
            .Returns(refreshToken);

        //Act
        var actual = await _sut.Handle(_request);

        //Assert
        actual.IsSuccess.ShouldBeTrue();
        actual.AccessToken.ShouldBe(accessToken);
        actual.RefreshToken.ShouldBe(refreshToken);
    }
}