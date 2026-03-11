using Absence.Application.Common.Interfaces;
using Absence.Application.Identity;
using Absence.Application.UseCases.Invitations.Commands;
using Absence.Application.UseCases.Invitations.Handlers;
using Absence.Domain.Entities;
using Absence.Domain.Interfaces;
using Absence.Domain.Specifications;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Shouldly;

namespace Absence.Application.Tests.UseCases.Invitations;

public class InviteUserToOrganizationHandlerTests
{
    private readonly IRepository<UserOrganizationRoleEntity> _userOrganizationRoleRepository;
    private readonly IRepository<OrganizationUserInvitationEntity> _invitationRepository;
    private readonly IUserService _userService;
    private readonly IUser _user;
    private InviteUserToOrganizationHandler _sut;

    public InviteUserToOrganizationHandlerTests()
    {
        _userOrganizationRoleRepository = Substitute.For<IRepository<UserOrganizationRoleEntity>>();
        _invitationRepository = Substitute.For<IRepository<OrganizationUserInvitationEntity>>();
        _userService = Substitute.For<IUserService>();
        _user = Substitute.For<IUser>();
        _sut = new InviteUserToOrganizationHandler(
            _userOrganizationRoleRepository,
            _invitationRepository,
            _userService,
            _user
        );
    }

    [Fact]
    public async Task Handle_ReturnsBadRequest_WhenInvitedUserDoesntExist()
    {
        //Arrange
        var request = new InviteUserToOrganizationCommand(
            1,
            new() { OrganizationId = 1, UserEmail = "email" }
        );

        _userService.FindByEmailAsync(request.Invite.UserEmail).ReturnsNull();

        //Act
        var actual = await _sut.Handle(request);

        //Assert
        actual.IsT1.ShouldBeTrue();
    }

    [Fact]
    public async Task Handle_ReturnsBadRequest_WhenUserAlreadyBelongsToOrganization()
    {
        //Arrange
        var request = new InviteUserToOrganizationCommand(
            1,
            new() { OrganizationId = 1, UserEmail = "email" }
        );

        var user = new UserEntity { FirstName = "first", LastName = "last" };
        _userService.FindByEmailAsync(request.Invite.UserEmail).Returns(user);

        _userOrganizationRoleRepository.AnyAsync(Arg.Any<UserRoleSpec>()).Returns(true);

        //Act
        var actual = await _sut.Handle(request);

        //Assert
        actual.IsT1.ShouldBeTrue();
    }

    [Fact]
    public async Task Handle_ReturnsBadRequest_WhenInvitationAlreadySent()
    {
        //Arrange
        var request = new InviteUserToOrganizationCommand(
            1,
            new() { OrganizationId = 1, UserEmail = "email" }
        );

        var user = new UserEntity { FirstName = "first", LastName = "last" };
        _userService.FindByEmailAsync(request.Invite.UserEmail).Returns(user);

        _userOrganizationRoleRepository.AnyAsync(Arg.Any<UserRoleSpec>()).Returns(false);

        _invitationRepository.AnyAsync(Arg.Any<InvitationSpec>()).Returns(true);

        //Act
        var actual = await _sut.Handle(request);

        //Assert
        actual.IsT1.ShouldBeTrue();
    }

    [Fact]
    public async Task Handle_ReturnsSuccess_WhenInvitationAddedSuccessfully()
    {
        //Arrange
        var request = new InviteUserToOrganizationCommand(
            1,
            new() { OrganizationId = 1, UserEmail = "email" }
        );

        var user = new UserEntity { ShortId = 3, FirstName = "first", LastName = "last" };
        _userService.FindByEmailAsync(request.Invite.UserEmail).Returns(user);

        _userOrganizationRoleRepository.AnyAsync(Arg.Any<UserRoleSpec>()).Returns(false);

        _invitationRepository.AnyAsync(Arg.Any<InvitationSpec>()).Returns(false);

        var inviterId = 2;
        _user.ShortId.Returns(inviterId);

        //Act
        var actual = await _sut.Handle(request);

        //Assert
        actual.IsT0.ShouldBeTrue();
        await _invitationRepository
            .Received()
            .InsertAsync(
                Arg.Is<OrganizationUserInvitationEntity>(x =>
                    x.Invited == user.ShortId &&
                    x.Inviter == inviterId &&
                    x.OrganizationId == request.OrganizationId
                )
            );
    }
}