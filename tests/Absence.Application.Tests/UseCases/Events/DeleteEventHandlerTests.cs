using Absence.Application.Common.Interfaces;
using Absence.Application.UseCases.Events.Commands;
using Absence.Application.UseCases.Events.Handlers;
using Absence.Domain.Entities;
using Absence.Domain.Interfaces;
using Absence.Domain.Specifications;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Shouldly;

namespace Absence.Application.Tests.UseCases.Events;

public class DeleteEventHandlerTests
{
    private DeleteEventCommand _command;
    private IRepository<EventEntity> _eventRepository;
    private IRepository<UserOrganizationRoleEntity> _userOrganizationRoleRepository;
    private IUser _user;
    private DeleteEventHandler _sut;

    public DeleteEventHandlerTests()
    {
        _command = new(1);
        _eventRepository = Substitute.For<IRepository<EventEntity>>();
        _userOrganizationRoleRepository = Substitute.For<IRepository<UserOrganizationRoleEntity>>();
        _user = Substitute.For<IUser>();
        _sut = new DeleteEventHandler(
            _eventRepository,
            _userOrganizationRoleRepository,
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
        _eventRepository.Received(0).Delete(Arg.Any<EventEntity>());
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
        _eventRepository.Received(0).Delete(Arg.Any<EventEntity>());
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
        _eventRepository.Received(1).Delete(@event);
        await _eventRepository.Received(1).SaveAsync();
    }
}