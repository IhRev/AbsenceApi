using Absence.Application.UseCases.Events.Commands;
using Absence.Domain.Entities;
using Absence.Domain.Interfaces;
using AutoMapper;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Absence.Application.UseCases.Events.Handlers;

public class EditEventHandler(IRepository<EventEntity> eventRepository, IMapper mapper) 
    : IRequestHandler<EditEventCommand, OneOf<Success, NotFound>>
{
    public async Task<OneOf<Success, NotFound>> Handle(
        EditEventCommand request, 
        CancellationToken cancellationToken = default
    )
    {
        var @event = await eventRepository.GetByIdAsync(request.Event.Id);
        if (@event is null)
        {
            return new NotFound();
        }

        mapper.Map(request.Event, @event);
        eventRepository.Update(@event);
        await eventRepository.SaveAsync(cancellationToken);

        return new Success();
    }
}