using Absence.Application.Common.Results;
using MediatR;
using OneOf.Types;
using OneOf;
using Absence.Application.UseCases.Events.DTOs;

namespace Absence.Application.UseCases.Events.Commands;

public record AddEventCommand(CreateEventDTO Event) : IRequest<OneOf<Success<int>, AccessDenied>>;