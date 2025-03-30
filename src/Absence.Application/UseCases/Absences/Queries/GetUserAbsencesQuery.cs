using Absence.Application.Common.DTOs;
using MediatR;

namespace Absence.Application.UseCases.Absences.Queries;

public class GetUserAbsencesQuery(string userId) : IRequest<IEnumerable<AbsenceDTO>>
{
    public string UserId { get; } = userId;
}