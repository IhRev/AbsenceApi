using Absence.Application.Common.Results;
using Absence.Application.UseCases.Holidays.DTOs;
using MediatR;
using OneOf.Types;
using OneOf;

namespace Absence.Application.UseCases.Holidays.Queries;

public record GetHolidaysQuery(int OrganizationId, DateTime StartDate, DateTime EndDate) : IRequest<OneOf<Success<IEnumerable<HolidayDTO>>, BadRequest>>;