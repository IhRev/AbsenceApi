using Absence.Application.Common.Results;
using Absence.Application.UseCases.Absences.DTOs;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Absence.Application.UseCases.Absences.Queries;

public class GetAbsenceEventsQuery(int organizationId) : IRequest<OneOf<Success<IEnumerable<AbsenceEventDTO>>, BadRequest, AccessDenied>>
{
    public int OrganizationId { get; } = organizationId;
}