using MediatR;
using OneOf;
using OneOf.Types;

namespace Absence.Application.UseCases.Users.Commands;

public record LogoutUserCommand : IRequest<OneOf<Success, NotFound>>;