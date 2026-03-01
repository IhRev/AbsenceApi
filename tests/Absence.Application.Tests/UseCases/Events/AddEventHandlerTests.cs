using Absence.Application.UseCases.Events.Commands;
using Absence.Application.UseCases.Events.Handlers;
using Absence.Domain.Entities;
using Absence.Domain.Interfaces;
using AutoMapper;
using NSubstitute;
using Shouldly;

namespace Absence.Application.Tests.UseCases.Events;

public class AddEventHandlerTests
{
    private AddEventCommand _command;
    private IRepository<EventEntity> _eventRepository;
    private IMapper _mapper;
    private AddEventHandler _sut;

    public AddEventHandlerTests()
    {
        _command = new(1, new() { Name = "name", Date = DateTime.Now });
        _eventRepository = Substitute.For<IRepository<EventEntity>>();
        _mapper = Substitute.For<IMapper>();
        _sut = new(_eventRepository, _mapper);
    }

    [Fact]
    public async Task Handle_ShouldReturnId()
    {
        // Arrange
        var entity = new EventEntity { Id = 2, Name = "name" };
        _mapper.Map<EventEntity>(_command.Event).Returns(entity);

        // Act
        var actual = await _sut.Handle(_command);

        // Assert
        actual.ShouldBe(entity.Id);
        await _eventRepository.Received(1).InsertAsync(entity);
        await _eventRepository.Received(1).SaveAsync();
    }
}