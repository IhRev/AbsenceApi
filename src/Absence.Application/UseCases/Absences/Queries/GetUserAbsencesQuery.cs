using Absence.Application.Common.DTOs;
using MediatR;

namespace Absence.Application.UseCases.Absences.Queries;

public class GetUserAbsencesQuery : IRequest<IEnumerable<AbsenceDTO>>;