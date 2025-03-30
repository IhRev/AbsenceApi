using Absence.Application.Common.DTOs;
using MediatR;

namespace Absence.Application.UseCases.Absences.Queries;

public class GetUserAbsencesQuery(int userId) : IRequest<IEnumerable<AbsenceDTO>>
{
    public int UserId { get; } = userId;
}