using Absence.Application.Common.Results;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Absence.Application.UseCases.Absences.Commands;

public record RespondAbsenceEventCommand(int Id, bool Accepted) : IRequest<OneOf<Success, NotFound, AccessDenied>>;