using MediatR;
using OneOf.Types;
using OneOf;
using Absence.Application.Common.Results;

namespace Absence.Application.UseCases.Holidays.Commands;

public record DeleteEventCommand(int Id) : IRequest<OneOf<Success, NotFound, AccessDenied>>;