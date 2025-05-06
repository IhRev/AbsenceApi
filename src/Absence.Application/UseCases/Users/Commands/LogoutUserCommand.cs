using MediatR;
using OneOf;
using OneOf.Types;

namespace Absence.Application.UseCases.Users.Commands;

public class LogoutUserCommand : IRequest<OneOf<Success, NotFound>>;