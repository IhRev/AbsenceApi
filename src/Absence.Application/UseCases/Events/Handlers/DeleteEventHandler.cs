using Absence.Application.Common.Constants;
using Absence.Application.Common.Interfaces;
using Absence.Application.Common.Results;
using Absence.Application.UseCases.Events.Commands;
using Absence.Domain.Entities;
using Absence.Domain.Extensions;
using Absence.Domain.Interfaces;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Absence.Application.UseCases.Events.Handlers;

public class DeleteEventHandler(
    IRepository<EventEntity> eventRepository,
    IRepository<UserOrganizationRoleEntity> userOrganizationRoleRepository,
    IUser user
) : IRequestHandler<DeleteEventCommand, OneOf<Success, NotFound, AccessDenied>>
{
    public async Task<OneOf<Success, NotFound, AccessDenied>> Handle(DeleteEventCommand request, CancellationToken cancellationToken)
    {
        var @event = await eventRepository.GetByIdAsync(request.Id);
        if (@event is null)
        {
            return new NotFound();
        }

        if (!await userOrganizationRoleRepository.HasPermission(@event.OrganizationId, user.ShortId, Permissions.MANAGE_EVENTS, cancellationToken))
        {
            return new AccessDenied();
        }

        eventRepository.Delete(@event);
        await eventRepository.SaveAsync(cancellationToken);

        return new Success();
    }
}