using Absence.Application.Common.Interfaces;
using Absence.Application.UseCases.Events.Commands;
using Absence.Application.UseCases.Events.Handlers;
using Absence.Domain.Entities;
using Absence.Domain.Interfaces;
using Absence.Domain.Specifications;
using AutoMapper;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Shouldly;

namespace Absence.Application.Tests.UseCases.Events;

public class EditEventHandlerTests
{
    private EditEventCommand _command;
    private IRepository<EventEntity> _eventRepository;
    private IRepository<UserOrganizationRoleEntity> _userOrganizationRoleRepository;
    private IMapper _mapper;
    private IUser _user;
    private EditEventHandler _sut;

    public EditEventHandlerTests()
    {
        _command = new(new() { Date = DateTime.Now, Name = "name" });
        _eventRepository = Substitute.For<IRepository<EventEntity>>();
        _userOrganizationRoleRepository = Substitute.For<IRepository<UserOrganizationRoleEntity>>();
        _user = Substitute.For<IUser>();
        _mapper = Substitute.For<IMapper>();
        _sut = new EditEventHandler(
            _eventRepository,
            _userOrganizationRoleRepository,
            _mapper,
            _user
        );
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenEventDoesntExist()
    {
        // Arrange
        _eventRepository.GetByIdAsync(Arg.Any<int>()).ReturnsNull();

        // Act
        var result = await _sut.Handle(_command, default);

        // Assert
        result.IsT1.ShouldBeTrue();
        _userOrganizationRoleRepository.Received(0);
        _eventRepository.Received(0).Update(Arg.Any<EventEntity>());
        await _eventRepository.Received(0).SaveAsync();
    }

    [Fact]
    public async Task Handle_ShouldReturnAcessDenied_WhenUserHasntPermission()
    {
        // Arrange
        var @event = new EventEntity { Date = DateTime.Now, Name = "name" };
        _eventRepository.GetByIdAsync(Arg.Any<int>()).Returns(@event);

        _userOrganizationRoleRepository.AnyAsync(Arg.Any<PermissionSpec>()).Returns(false);

        // Act
        var result = await _sut.Handle(_command, default);

        // Assert
        result.IsT2.ShouldBeTrue();
        _eventRepository.Received(0).Update(@event);
        await _eventRepository.Received(0).SaveAsync();
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenUserHasPermission()
    {
        // Arrange
        var @event = new EventEntity { Date = DateTime.Now, Name = "name" };
        _eventRepository.GetByIdAsync(Arg.Any<int>()).Returns(@event);

        _userOrganizationRoleRepository.AnyAsync(Arg.Any<PermissionSpec>()).Returns(true);

        // Act
        var result = await _sut.Handle(_command, default);

        // Assert
        result.IsT0.ShouldBeTrue();
        _mapper.Received(1).Map(_command.Event, @event);
        _eventRepository.Received(1).Update(@event);
        await _eventRepository.Received(1).SaveAsync();
    }
}