using MediatR;

namespace Absence.Application.UseCases.Users.Commands;

public record LogoutUserCommand : IRequest;