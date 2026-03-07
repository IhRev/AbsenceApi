using Absence.Application.Common.DTOs;
using MediatR;
using OneOf;
using OneOf.Types;

namespace Absence.Application.UseCases.Users.Commands;

public record UpdateUserCommand(UserDetails UserDetails) : IRequest<OneOf<Success, NotFound>>;