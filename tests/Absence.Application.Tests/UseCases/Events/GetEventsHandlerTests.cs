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
    private IMapper _mapper;
    private GetEventsHandler _sut;

    public GetEventsHandlerTests()
    {
        _query = new(1, DateTime.Today.AddDays(-1), DateTime.Today.AddDays(1));
        _eventRepository = Substitute.For<IRepository<EventEntity>>();
        _mapper = Substitute.For<IMapper>();
        _sut = new(_eventRepository, _mapper);
    }

    [Fact]
    public async Task Handle_ShouldReturnEvents()
    {
        // Arrange
        var entities = new List<EventEntity>()
        {
            new() { Id = 1, Name = "name" }
        };
        _eventRepository.GetAsync(Arg.Any<EventsSpec>()).Returns(entities);

        var dtos = new List<EventDTO>()
        {
            new() { Name = "name1" },
            new() { Name = "name2" }
        };
        _mapper.Map<IEnumerable<EventDTO>>(entities).Returns(dtos);

        // Act
        var actual = await _sut.Handle(_query);

        // Assert
        actual.ShouldBe(dtos);
    }
}