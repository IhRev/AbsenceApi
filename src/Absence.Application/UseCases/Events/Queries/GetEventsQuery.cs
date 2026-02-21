using Absence.Application.Common.Results;
using Absence.Application.UseCases.Holidays.DTOs;
using MediatR;
using OneOf.Types;
using OneOf;

namespace Absence.Application.UseCases.Holidays.Queries;

public record GetEventsQuery(int OrganizationId, DateTime StartDate, DateTime EndDate) : IRequest<OneOf<Success<IEnumerable<HolidayDTO>>, AccessDenied>>;