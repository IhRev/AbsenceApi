using Absence.Application.Common.Results;
using Absence.Application.UseCases.Absences.DTOs;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Absence.Application.UseCases.Absences.Queries;

public class GetUserAbsencesQuery(DateTimeOffset startDate, DateTimeOffset endDate, int organizationId) 
    : IRequest<OneOf<Success<IEnumerable<AbsenceDTO>>, BadRequest>>
{
    public DateTimeOffset StartDate { get; } = startDate;
    public DateTimeOffset EndDate { get; } = endDate;
    public int OrganizationId { get; } = organizationId;
}