using Absence.Application.UseCases.Events.Commands;
using Absence.Domain.Entities;
using Absence.Domain.Interfaces;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Absence.Application.UseCases.Events.Handlers;

public class DeleteEventHandler(IRepository<EventEntity> eventRepository) 
    : IRequestHandler<DeleteEventCommand, OneOf<Success, NotFound>>
{
    public async Task<OneOf<Success, NotFound>> Handle(
        DeleteEventCommand request, 
        CancellationToken cancellationToken = default
    )
    {
        var @event = await eventRepository.GetByIdAsync(request.Id);
        if (@event is null)
        {
            return new NotFound();
        }

        eventRepository.Delete(@event);
        await eventRepository.SaveAsync(cancellationToken);

        return new Success();
    }
}