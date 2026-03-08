using Absence.Application.Common.Interfaces;
using Absence.Application.UseCases.Invitations.Commands;
using Absence.Application.UseCases.Invitations.Handlers;
using Absence.Domain.Entities;
using Absence.Domain.Interfaces;
using Absence.Domain.Specifications;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Shouldly;

namespace Absence.Application.Tests.UseCases.Invitations;

public class AcceptInvitationHandlerTests
{
    private readonly IRepository<OrganizationUserInvitationEntity> _organizationUserInvitationRepository;
    private readonly IRepository<UserOrganizationRoleEntity> _userOrganizationRoleRepository;
    private readonly IRepository<OrganizationRoleEntity> _organizationRoleRepository;
    private readonly IUser _user;
    private readonly AcceptInvitationHandler _sut;

    public AcceptInvitationHandlerTests()
    {
        _organizationUserInvitationRepository = Substitute.For<IRepository<OrganizationUserInvitationEntity>>();
        _userOrganizationRoleRepository = Substitute.For<IRepository<UserOrganizationRoleEntity>>();
        _organizationRoleRepository = Substitute.For<IRepository<OrganizationRoleEntity>>();
        _user = Substitute.For<IUser>();
        _sut = new AcceptInvitationHandler(
            _organizationUserInvitationRepository,
            _userOrganizationRoleRepository,
            _organizationRoleRepository,
            _user
        );
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenInvitationDoesNotExist()
    {
        //Arrange
        var request = new AcceptInvitationCommand(1, true);

        _organizationUserInvitationRepository
            .GetByIdAsync(Arg.Any<int>())
            .ReturnsNull();

        //Act
        var result = await _sut.Handle(request);

        //Assert
        result.IsT1.ShouldBeTrue();
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenInvitationDoesntBelongToUser()
    {
        //Arrange
        var request = new AcceptInvitationCommand(1, true);

        var invitation = new OrganizationUserInvitationEntity
        {
            Id = 1,
            Invited = 2
        };
        _organizationUserInvitationRepository
            .GetByIdAsync(Arg.Any<int>())
            .Returns(invitation);

        _user.ShortId.Returns(3);

        //Act
        var result = await _sut.Handle(request);

        //Assert
        result.IsT1.ShouldBeTrue();
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccessAndAddViewerRole_WhenUserAccepted()
    {
        //Arrange
        var request = new AcceptInvitationCommand(1, true);

        var invitation = new OrganizationUserInvitationEntity
        {
            Id = 1,
            Invited = 2,
            OrganizationId = 3
        };
        _organizationUserInvitationRepository
            .GetByIdAsync(Arg.Any<int>())
            .Returns(invitation);

        _user.ShortId.Returns(invitation.Invited);

        var roleId = 4;
        _organizationRoleRepository
            .GetFirstOrDefaultAsync(Arg.Any<RoleSpec>())
            .Returns(new OrganizationRoleEntity { Name = "role", Id = roleId });

        //Act
        var result = await _sut.Handle(request);

        //Assert
        result.IsT0.ShouldBeTrue();

        await _userOrganizationRoleRepository
            .Received()
            .InsertAsync(
                Arg.Is<UserOrganizationRoleEntity>(x =>
                    x.OrganizationRoleId == roleId &&
                    x.UserId == invitation.Invited &&
                    x.OrganizationId == invitation.OrganizationId
                )
            );
        await _userOrganizationRoleRepository
            .Received()
            .SaveAsync();

        _organizationUserInvitationRepository
            .Received()
            .Delete(invitation);
        await _organizationUserInvitationRepository
            .Received()
            .SaveAsync();
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccessWithoutAddingRole_WhenUserRejected()
    {
        //Arrange
        var request = new AcceptInvitationCommand(1, false);

        var invitation = new OrganizationUserInvitationEntity
        {
            Id = 1,
            Invited = 2,
            OrganizationId = 3
        };
        _organizationUserInvitationRepository
            .GetByIdAsync(Arg.Any<int>())
            .Returns(invitation);

        _user.ShortId.Returns(invitation.Invited);

        //Act
        var result = await _sut.Handle(request);

        //Assert
        result.IsT0.ShouldBeTrue();

        await _userOrganizationRoleRepository
            .DidNotReceive()
            .InsertAsync(Arg.Any<UserOrganizationRoleEntity>());

        _organizationUserInvitationRepository
            .Received()
            .Delete(invitation);
        await _organizationUserInvitationRepository
            .Received()
            .SaveAsync();
    }
}