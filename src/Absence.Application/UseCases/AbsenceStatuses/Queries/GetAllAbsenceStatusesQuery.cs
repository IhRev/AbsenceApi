using Absence.Application.UseCases.AbsenceStatuses.DTOs;
using MediatR;

namespace Absence.Application.UseCases.AbsenceStatuses.Queries;

public class GetAllAbsenceStatusesQuery : IRequest<IEnumerable<AbsenceStatusDTO>>;