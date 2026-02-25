using Absence.Application.UseCases.Events.DTOs;
using Absence.Application.UseCases.Events.Queries;
using Absence.Domain.Entities;
using Absence.Domain.Interfaces;
using Absence.Domain.Specifications;
using AutoMapper;
using MediatR;

namespace Absence.Application.UseCases.Events.Handlers;

public class GetEventsHandler(IRepository<EventEntity> eventRepository, IMapper mapper) 
    : IRequestHandler<GetEventsQuery, IEnumerable<EventDTO>>
{
    public async Task<IEnumerable<EventDTO>> Handle(
        GetEventsQuery request, 
        CancellationToken cancellationToken = default
    )
    {
        var events = await eventRepository.GetAsync(
            new EventsSpec(request.OrganizationId, request.StartDate, request.EndDate)
        );
        return mapper.Map<IEnumerable<EventDTO>>(events);
    }
}