using Absence.Application.Common.Interfaces;
using Absence.Application.Identity;
using Absence.Application.UseCases.Users.Commands;
using Absence.Application.UseCases.Users.Handlers;
using Absence.Domain.Entities;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Shouldly;
using System.Security.Claims;

namespace Absence.Application.Tests.UseCases.Users;

public class RefreshTokenHandlerTests
{
    private const string USER_ID = "userId";
    private readonly RefreshTokenCommand _request = 
        new(new() { AccessToken = "access", RefreshToken = "refresh" });
    private readonly IUserService _userService;
    private readonly IJwtService _jwtService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly RefreshTokenHandler _sut;

    public RefreshTokenHandlerTests()
    {
        _userService = Substitute.For<IUserService>();
        _jwtService = Substitute.For<IJwtService>();
        _refreshTokenService = Substitute.For<IRefreshTokenService>();
        _dateTimeProvider = Substitute.For<IDateTimeProvider>();
        _sut = new(_userService, _jwtService, _refreshTokenService, _dateTimeProvider);
    }

    [Fact]
    public async Task Handle_ReturnsFail_WhenUserDoesntExist()
    {
        //Arrange
        _jwtService
            .GetPrincipalFromExpiredToken(_request.RefreshTokenRequest.AccessToken)
            .Returns(new ClaimsPrincipal(new ClaimsIdentity([new(ClaimTypes.NameIdentifier, USER_ID)])));

        _userService.FindByIdAsync(USER_ID).ReturnsNull();

        //Act
        var actual = await _sut.Handle(_request);

        //Assert
        actual.IsSuccess.ShouldBeFalse();
        _jwtService.DidNotReceive().GenerateToken(Arg.Any<UserEntity>());
    }

    [Fact]
    public async Task Handle_ReturnsFail_WhenRefreshTokenIsIncorrect()
    {
        //Arrange
        _jwtService
            .GetPrincipalFromExpiredToken(_request.RefreshTokenRequest.AccessToken)
            .Returns(new ClaimsPrincipal(new ClaimsIdentity([new(ClaimTypes.NameIdentifier, USER_ID)])));

        var user = new UserEntity
        {
            FirstName = "first",
            LastName = "last",
            RefreshToken = "abc"
        };
        _userService.FindByIdAsync(USER_ID).Returns(user);

        //Act
        var actual = await _sut.Handle(_request);

        //Assert
        actual.IsSuccess.ShouldBeFalse();
        _jwtService.DidNotReceive().GenerateToken(Arg.Any<UserEntity>());
    }

    [Fact]
    public async Task Handle_ReturnsFail_WhenRefreshTokenExpired()
    {
        //Arrange
        _jwtService
            .GetPrincipalFromExpiredToken(_request.RefreshTokenRequest.AccessToken)
            .Returns(new ClaimsPrincipal(new ClaimsIdentity([new(ClaimTypes.NameIdentifier, USER_ID)])));

        var user = new UserEntity
        {
            FirstName = "first",
            LastName = "last",
            RefreshToken = _request.RefreshTokenRequest.RefreshToken,
            RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(-1)
        };
        _userService.FindByIdAsync(USER_ID).Returns(user);

        _dateTimeProvider.UtcNow.Returns(DateTime.UtcNow);

        //Act
        var actual = await _sut.Handle(_request);

        //Assert
        actual.IsSuccess.ShouldBeFalse();
        _jwtService.DidNotReceive().GenerateToken(Arg.Any<UserEntity>());
    }

    [Fact]
    public async Task Handle_ReturnsSuccess_WhenTokenRefreshedSuccessfully()
    {
        //Arrange
        _jwtService
            .GetPrincipalFromExpiredToken(_request.RefreshTokenRequest.AccessToken)
            .Returns(new ClaimsPrincipal(new ClaimsIdentity([new(ClaimTypes.NameIdentifier, USER_ID)])));

        _dateTimeProvider.UtcNow.Returns(DateTime.UtcNow);

        var user = new UserEntity
        {
            FirstName = "first",
            LastName = "last",
            RefreshToken = _request.RefreshTokenRequest.RefreshToken,
            RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(1)
        };
        _userService.FindByIdAsync(USER_ID).Returns(user);

        var newAccessToken = "newAccess";
        _jwtService.GenerateToken(user).Returns(newAccessToken);

        var newRefreshToken = "newRefresh";
        _refreshTokenService.GenerateToken(user).Returns(newRefreshToken);

        //Act
        var actual = await _sut.Handle(_request);

        //Assert
        actual.IsSuccess.ShouldBeTrue();
        actual.AccessToken.ShouldBe(newAccessToken);
        actual.RefreshToken.ShouldBe(newRefreshToken);
    }
}