using Absence.Application.UseCases.AbsenceEventTypes.DTOs;
using MediatR;

namespace Absence.Application.UseCases.AbsenceEventTypes.Queries;

public class GetAllAbsenceEventTypesQuery : IRequest<IEnumerable<AbsenceEventTypeDTO>>;