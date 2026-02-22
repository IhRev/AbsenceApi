using Absence.Application.Common.Results;
using MediatR;
using OneOf.Types;
using OneOf;
using Absence.Application.UseCases.Events.DTOs;

namespace Absence.Application.UseCases.Events.Queries;

public record GetEventsQuery(int OrganizationId, DateTime StartDate, DateTime EndDate) : IRequest<OneOf<Success<IEnumerable<EventDTO>>, AccessDenied>>;