using Absence.Application.Common.Results;
using Absence.Application.UseCases.Absences.DTOs;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Absence.Application.UseCases.Absences.Queries;

public class GetUserAbsencesQuery(DateTime startDate, DateTime endDate, int organizationId) 
    : IRequest<OneOf<Success<IEnumerable<AbsenceDTO>>, BadRequest>>
{
    public DateTime StartDate { get; } = startDate;
    public DateTime EndDate { get; } = endDate;
    public int OrganizationId { get; } = organizationId;
}