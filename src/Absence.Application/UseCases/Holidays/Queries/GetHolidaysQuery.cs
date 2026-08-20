using Absence.Application.Common.Results;
using Absence.Application.UseCases.Holidays.DTOs;
using MediatR;
using OneOf.Types;
using OneOf;

namespace Absence.Application.UseCases.Holidays.Queries;

public class GetHolidaysQuery(int organizationId, DateTimeOffset startDate, DateTimeOffset endDate) : IRequest<OneOf<Success<IEnumerable<HolidayDTO>>, BadRequest>>
{
    public int OrganizationId { get; } = organizationId;
    public DateTimeOffset StartDate { get; } = startDate;
    public DateTimeOffset EndDate { get; } = endDate;
}