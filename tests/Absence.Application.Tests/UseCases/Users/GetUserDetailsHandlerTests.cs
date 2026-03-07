using Absence.Application.Common.DTOs;
using Absence.Application.Common.Interfaces;
using Absence.Application.Identity;
using Absence.Application.UseCases.Users.Handlers;
using Absence.Domain.Entities;
using AutoMapper;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Shouldly;

namespace Absence.Application.Tests.UseCases.Users;

public class GetUserDetailsHandlerTests
{
    private IUser _user;
    private IUserService _userService;
    private IMapper _mapper;
    private GetUserDetailsHandler _sut;

    public GetUserDetailsHandlerTests()
    {
        _user = Substitute.For<IUser>();
        _userService = Substitute.For<IUserService>();
        _mapper = Substitute.For<IMapper>();
        _sut = new GetUserDetailsHandler(_user, _userService, _mapper);
    }

    [Fact]
    public async Task Handle_ReturnsResult_WhenUserExist()
    {
        //Arrange
        var entity = new UserEntity
        {
            FirstName = "fname",
            LastName = "lname"
        };
        _userService.FindByIdAsync(Arg.Any<string>()).Returns(entity);

        var expected = new UserDetails
        {
            FirstName = "first",
            LastName = "last",
            Email = "email"
        };
        _mapper.Map<UserDetails>(entity).Returns(expected);

        //Act
        var actual = await _sut.Handle(new());

        //Assert
        actual.IsT0.ShouldBeTrue();
        actual.AsT0.ShouldBe(expected);
    }

    [Fact]
    public async Task Handle_ReturnsNotFound_WhenUserDoesntExist()
    {
        //Arrange
        _userService.FindByIdAsync(Arg.Any<string>()).ReturnsNull();

        //Act
        var actual = await _sut.Handle(new());

        //Assert
        actual.IsT1.ShouldBeTrue();
        _mapper.DidNotReceive().Map<UserDetails>(Arg.Any<UserEntity>());
    }
}