using Absence.Application.Common.Results;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Absence.Application.UseCases.AbsenceTypes.Commands;

public record DeleteAbsenceTypeCommand(int Id) : IRequest<OneOf<Success, NotFound, AccessDenied>>;