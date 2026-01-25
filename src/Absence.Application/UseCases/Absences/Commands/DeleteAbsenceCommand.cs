using Absence.Application.Common.Results;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Absence.Application.UseCases.Absences.Commands;

public record DeleteAbsenceCommand(int Id) : IRequest<OneOf<Success<string>, NotFound, AccessDenied>>;