using Absence.Application.Common.Results;
using Absence.Application.UseCases.Absences.DTOs;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Absence.Application.UseCases.Absences.Queries;

public class GetUsersAbsencesQuery(DateTimeOffset startDate, DateTimeOffset endDate, int organizationId, List<int> userIds)
    : IRequest<OneOf<Success<IEnumerable<AbsenceDTO>>, BadRequest, AccessDenied>>
{
    public DateTimeOffset StartDate { get; } = startDate;
    public DateTimeOffset EndDate { get; } = endDate;
    public int OrganizationId { get; } = organizationId;
    public List<int> UserIds { get; } = userIds;
}