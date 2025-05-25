using Absence.Application.UseCases.Absences.DTOs;
using MediatR;

namespace Absence.Application.UseCases.Absences.Queries;

public class GetUserAbsencesQuery(DateTime startDate, DateTime endDate) : IRequest<IEnumerable<AbsenceDTO>>
{
    public DateTime StartDate { get; } = startDate;
    public DateTime EndDate { get; } = endDate;
}