using MediatR;
using Absence.Application.UseCases.Events.DTOs;
using Absence.Application.Common.Interfaces;
using Absence.Application.Common.Constants;

namespace Absence.Application.UseCases.Events.Queries;

public record GetEventsQuery(int OrganizationId, DateTime StartDate, DateTime EndDate)
    : IRequest<IEnumerable<EventDTO>>, IRequirePermission
{
    public string Permission => Permissions.VIEW_BASICS;
}