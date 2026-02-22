using Absence.Application.Common.Constants;
using Absence.Application.Common.Interfaces;
using Absence.Application.Common.Results;
using Absence.Application.UseCases.Events.DTOs;
using Absence.Application.UseCases.Events.Queries;
using Absence.Domain.Entities;
using Absence.Domain.Extensions;
using Absence.Domain.Interfaces;
using Absence.Domain.Specifications;
using AutoMapper;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Absence.Application.UseCases.Events.Handlers;

public class GetEventsHandler(
    IRepository<EventEntity> eventRepository,
    IRepository<UserOrganizationRoleEntity> userOrganizationRoleRepository,
    IMapper mapper,
    IUser user
) : IRequestHandler<GetEventsQuery, OneOf<Success<IEnumerable<EventDTO>>, AccessDenied>>
{
    public async Task<OneOf<Success<IEnumerable<EventDTO>>, AccessDenied>> Handle(GetEventsQuery request, CancellationToken cancellationToken)
    {
        if (!await userOrganizationRoleRepository.HasPermission(request.OrganizationId, user.ShortId, Permissions.VIEW, cancellationToken))
        {
            return new AccessDenied();
        }

        var events = await eventRepository.GetAsync(new EventsSpec(request.OrganizationId, request.StartDate, request.EndDate));
        return new Success<IEnumerable<EventDTO>>(mapper.Map<IEnumerable<EventDTO>>(events));
    }
}