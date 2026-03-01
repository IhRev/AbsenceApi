using Absence.Application.UseCases.Events.Commands;
using Absence.Application.UseCases.Events.Handlers;
using Absence.Domain.Entities;
using Absence.Domain.Interfaces;
using AutoMapper;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Shouldly;

namespace Absence.Application.Tests.UseCases.Events;

public class EditEventHandlerTests
{
    private EditEventCommand _command;
    private IRepository<EventEntity> _eventRepository;
    private IMapper _mapper;
    private EditEventHandler _sut;

    public EditEventHandlerTests()
    {
        _command = new(1, new() { Date = DateTime.Now, Name = "name" });
        _eventRepository = Substitute.For<IRepository<EventEntity>>();
        _mapper = Substitute.For<IMapper>();
        _sut = new(_eventRepository, _mapper);
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
        _eventRepository.Received(0).Update(Arg.Any<EventEntity>());
        await _eventRepository.Received(0).SaveAsync();
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenEventDeleted()
    {
        // Arrange
        var @event = new EventEntity { Date = DateTime.Now, Name = "name" };
        _eventRepository.GetByIdAsync(Arg.Any<int>()).Returns(@event);

        // Act
        var actual = await _sut.Handle(_command);

        // Assert
        actual.IsT0.ShouldBeTrue();
        _mapper.Received(1).Map(_command.Event, @event);
        _eventRepository.Received(1).Update(@event);
        await _eventRepository.Received(1).SaveAsync();
    }
}