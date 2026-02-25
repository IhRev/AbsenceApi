using Absence.Application.UseCases.Events.Commands;
using Absence.Domain.Entities;
using Absence.Domain.Interfaces;
using AutoMapper;
using MediatR;

namespace Absence.Application.UseCases.Events.Handlers;

internal class AddEventHandler(IRepository<EventEntity> eventRepository, IMapper mapper) 
    : IRequestHandler<AddEventCommand, int>
{
    public async Task<int> Handle(AddEventCommand request, CancellationToken cancellationToken = default)
    {
        var @event = mapper.Map<EventEntity>(request.Event);
        await eventRepository.InsertAsync(@event, cancellationToken);
        await eventRepository.SaveAsync(cancellationToken);
        return @event.Id;
    }
}