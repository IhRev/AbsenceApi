using Absence.Application.Common.DTOs;
using Absence.Application.Common.Responses;
using MediatR;

namespace Absence.Application.UseCases.Users.Commands;

public class LoginUserCommand(UserCredentials credentials) : IRequest<AuthResponse>
{
    public UserCredentials Credentials { get; } = credentials;
}