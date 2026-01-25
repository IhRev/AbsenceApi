using Absence.Application.Common.Results;
using Absence.Application.UseCases.Absences.DTOs;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Absence.Application.UseCases.Absences.Queries;

public record GetUserAbsencesQuery(DateTime StartDate, DateTime EndDate, int OrganizationId) 
    : IRequest<OneOf<Success<IEnumerable<AbsenceDTO>>, BadRequest>>;