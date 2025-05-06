using Absence.Application.UseCases.Absences.DTOs;
using MediatR;

namespace Absence.Application.UseCases.Absences.Queries;

public class GetUserAbsencesQuery : IRequest<IEnumerable<AbsenceDTO>>;