using Absence.Application.Common.Results;
using Absence.Application.UseCases.Absences.DTOs;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Absence.Application.UseCases.Absences.Queries;

public class GetUsersAbsencesQuery(DateTime startDate, DateTime endDate, int organizationId, List<int> userIds)
    : IRequest<OneOf<Success<IEnumerable<AbsenceDTO>>, BadRequest, AccessDenied>>
{
    public DateTime StartDate { get; } = startDate;
    public DateTime EndDate { get; } = endDate;
    public int OrganizationId { get; } = organizationId;
    public List<int> UserIds { get; } = userIds;
}