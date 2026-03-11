using Absence.Application.Common.Interfaces;
using Absence.Application.UseCases.Invitations.DTOs;
using Absence.Application.UseCases.Invitations.Handlers;
using Absence.Domain.Entities;
using Absence.Domain.Interfaces;
using Absence.Domain.Specifications;
using AutoMapper;
using NSubstitute;
using Shouldly;

namespace Absence.Application.Tests.UseCases.Invitations;

public class GetUserInvitationsHandlerTests
{
    private readonly IRepository<OrganizationUserInvitationEntity> _invitationRepository;
    private readonly IUser _user;
    private readonly IMapper _mapper;
    private readonly GetUserInvitationsHandler _sut;

    public GetUserInvitationsHandlerTests()
    {
        _invitationRepository = Substitute.For<IRepository<OrganizationUserInvitationEntity>>();
        _user = Substitute.For<IUser>();
        _mapper = Substitute.For<IMapper>();
        _sut = new(_invitationRepository, _user, _mapper);
    }

    [Fact]
    public async Task Handle_ReturnsResult()
    {
        //Arrange
        var invitations = new List<OrganizationUserInvitationEntity>
        {
            new() { Id = 1 },
            new() { Id = 2 }
        };
        _invitationRepository.GetAsync(Arg.Any<InvitationSpec>()).Returns(invitations);

        var expected = new List<InvitationDTO>
        {
            new() { Inviter = "inviter1", Organization = "org1" },
            new() { Inviter = "inviter2", Organization = "org2" },
        };
        _mapper.Map<IEnumerable<InvitationDTO>>(invitations).Returns(expected);

        //Act
        var actual = await _sut.Handle(new());

        //Assert
        actual.ShouldBe(expected);
    }
}