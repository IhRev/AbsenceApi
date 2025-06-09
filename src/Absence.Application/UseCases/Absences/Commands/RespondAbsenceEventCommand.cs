using Absence.Application.Common.Results;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Absence.Application.UseCases.Absences.Commands;

public class RespondAbsenceEventCommand(int id, bool accepted) : IRequest<OneOf<Success, NotFound, AccessDenied>>
{
    public int Id { get; } = id;
    public bool Accepted { get; } = accepted;
}