using Absence.Application.Common.Interfaces;
using Absence.Application.UseCases.Events.Commands;
using Absence.Application.UseCases.Events.Handlers;
using Absence.Domain.Entities;
using Absence.Domain.Interfaces;
using Absence.Domain.Specifications;
using AutoMapper;
using NSubstitute;
using Shouldly;

namespace Absence.Application.Tests.UseCases.Events;

public class AddHolidayHandlerTests
{
    private AddEventCommand _command;
    private IRepository<EventEntity> _eventRepository;
    private IRepository<UserOrganizationRoleEntity> _userOrganizationRoleRepository;
    private IMapper _mapper;
    private IUser _user;
    private AddEventHandler _sut;

    public AddHolidayHandlerTests()
    {
        _command = new(new() { OrganizationId = 1, Name = "name", Date = DateTime.Now });
        _eventRepository = Substitute.For<IRepository<EventEntity>>();
        _userOrganizationRoleRepository = Substitute.For<IRepository<UserOrganizationRoleEntity>>();
        _mapper = Substitute.For<IMapper>();
        _user = Substitute.For<IUser>();
        _sut = new AddEventHandler(
            _eventRepository,
            _userOrganizationRoleRepository,
            _mapper,
            _user
        );
    }

    [Fact]
    public async Task Handle_ShouldReturnAccessDenied_WhenUserHasntPermission()
    {
        // Arrange
        _userOrganizationRoleRepository.AnyAsync(Arg.Any<PermissionSpec>()).Returns(false);

        // Act
        var result = await _sut.Handle(_command, default);

        // Assert
        result.IsT1.ShouldBeTrue();
        _eventRepository.Received(0);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenUserHasPermission()
    {
        // Arrange
        _userOrganizationRoleRepository.AnyAsync(Arg.Any<PermissionSpec>()).Returns(true);

        var entity = new EventEntity { Date = DateTime.Now, Name = "name" };
        _mapper.Map<EventEntity>(_command.Event).Returns(entity);

        // Act
        var result = await _sut.Handle(_command, default);

        // Assert
        result.IsT0.ShouldBeTrue();
        await _eventRepository.Received(1).InsertAsync(entity);
        await _eventRepository.Received(1).SaveAsync();
    }
}