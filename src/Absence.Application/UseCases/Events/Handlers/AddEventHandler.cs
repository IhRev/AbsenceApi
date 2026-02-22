using Absence.Application.Common.Constants;
using Absence.Application.Common.Interfaces;
using Absence.Application.Common.Results;
using Absence.Application.UseCases.Events.Commands;
using Absence.Domain.Entities;
using Absence.Domain.Extensions;
using Absence.Domain.Interfaces;
using AutoMapper;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Absence.Application.UseCases.Events.Handlers;

internal class AddEventHandler(
    IRepository<EventEntity> eventRepository,
    IRepository<UserOrganizationRoleEntity> userOrganizationRoleRepository,
    IMapper mapper,
    IUser user
) : IRequestHandler<AddEventCommand, OneOf<Success<int>, AccessDenied>>
{
    public async Task<OneOf<Success<int>, AccessDenied>> Handle(AddEventCommand request, CancellationToken cancellationToken)
    {
        if (!await userOrganizationRoleRepository.HasPermission(request.Event.OrganizationId, user.ShortId, Permissions.MANAGE_EVENTS, cancellationToken))
        {
            return new AccessDenied();
        }

        var @event = mapper.Map<EventEntity>(request.Event);
        await eventRepository.InsertAsync(@event, cancellationToken);
        await eventRepository.SaveAsync(cancellationToken);
        return new Success<int>(@event.Id);
    }
}