using Absence.Application.Common.Interfaces;
using Absence.Application.UseCases.Events.DTOs;
using Absence.Application.UseCases.Events.Handlers;
using Absence.Application.UseCases.Events.Queries;
using Absence.Domain.Entities;
using Absence.Domain.Interfaces;
using Absence.Domain.Specifications;
using AutoMapper;
using NSubstitute;
using Shouldly;

namespace Absence.Application.Tests.UseCases.Events;

public class GetEventsHandlerTests
{
    private GetEventsQuery _query;
    private IRepository<EventEntity> _eventRepository;
    private IRepository<UserOrganizationRoleEntity> _userOrganizationRoleRepository;
    private IMapper _mapper;
    private IUser _user;
    private GetEventsHandler _sut;

    public GetEventsHandlerTests()
    {
        _query = new(1, DateTime.Today.AddDays(-1), DateTime.Today.AddDays(1));
        _eventRepository = Substitute.For<IRepository<EventEntity>>();
        _userOrganizationRoleRepository = Substitute.For<IRepository<UserOrganizationRoleEntity>>();
        _user = Substitute.For<IUser>();
        _mapper = Substitute.For<IMapper>();
        _sut = new GetEventsHandler(
            _eventRepository,
            _userOrganizationRoleRepository,
            _mapper,
            _user
        );
    }

    [Fact]
    public async Task Handle_ShouldReturnAccessDenied_WhenUserDoesNotHavePermission()
    {
        // Arrange
        _userOrganizationRoleRepository.AnyAsync(Arg.Any<PermissionSpec>()).Returns(false);
        
        // Act
        var actual = await _sut.Handle(_query, CancellationToken.None);
        
        // Assert
        actual.IsT1.ShouldBeTrue();
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenUserHasPermission()
    {
        // Arrange
        _userOrganizationRoleRepository.AnyAsync(Arg.Any<PermissionSpec>()).Returns(true);

        var entities = new List<EventEntity>();
        _eventRepository.GetAsync(Arg.Any<EventsSpec>()).Returns(entities);

        var dtos = new List<EventDTO>();
        _mapper.Map<IEnumerable<EventDTO>>(entities).Returns(dtos);

        // Act
        var actual = await _sut.Handle(_query, CancellationToken.None);

        // Assert
        actual.IsT0.ShouldBeTrue();
        actual.AsT0.Value.ShouldBe(dtos);
    }
}