using Absence.Application.Common.Results;
using Absence.Application.UseCases.Holidays.DTOs;
using MediatR;
using OneOf.Types;
using OneOf;

namespace Absence.Application.UseCases.Holidays.Queries;

public class GetHolidaysQuery(int organizationId, DateTime startDate, DateTime endDate) : IRequest<OneOf<Success<IEnumerable<HolidayDTO>>, BadRequest>>
{
    public int OrganizationId { get; } = organizationId;
    public DateTime StartDate { get; } = startDate;
    public DateTime EndDate { get; } = endDate;
}