using Absence.Application.Common.DTOs;
using Absence.Application.Common.Responses;
using MediatR;

namespace Absence.Application.UseCases.Users.Commands;

public class LoginUserCommand(UserCredentialsDTO credentials) : IRequest<AuthResponse>
{
    public UserCredentialsDTO Credentials { get; } = credentials;
}