using Absence.Application.UseCases.Events.Commands;
using Absence.Application.UseCases.Events.Handlers;
using Absence.Domain.Entities;
using Absence.Domain.Interfaces;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Shouldly;

namespace Absence.Application.Tests.UseCases.Events;

public class DeleteEventHandlerTests
{
    private DeleteEventCommand _command;
    private IRepository<EventEntity> _eventRepository;
    private DeleteEventHandler _sut;

    public DeleteEventHandlerTests()
    {
        _command = new(1, 2);
        _eventRepository = Substitute.For<IRepository<EventEntity>>();
        _sut = new(_eventRepository);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenEventDoesntExist()
    {
        // Arrange
        _eventRepository.GetByIdAsync(Arg.Any<int>()).ReturnsNull();

        // Act
        var actual = await _sut.Handle(_command);

        // Assert
        actual.IsT1.ShouldBeTrue();
        _eventRepository.Received(0).Delete(Arg.Any<EventEntity>());
        await _eventRepository.Received(0).SaveAsync();
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenEventDeleted()
    {
        // Arrange
        var @event = new EventEntity { Name = "name" };
        _eventRepository.GetByIdAsync(Arg.Any<int>()).Returns(@event);

        // Act
        var actual = await _sut.Handle(_command);

        // Assert
        actual.IsT0.ShouldBeTrue();
        _eventRepository.Received(1).Delete(@event);
        await _eventRepository.Received(1).SaveAsync();
    }
}