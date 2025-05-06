using Absence.Application.UseCases.Users.DTOs;
using MediatR;

namespace Absence.Application.UseCases.Users.Commands;

public class LoginUserCommand(UserCredentials credentials) : IRequest<AuthResponse>
{
    public UserCredentials Credentials { get; } = credentials;
}