using Absence.Application.UseCases.Users.DTOs;
using MediatR;

namespace Absence.Application.UseCases.Users.Commands;

public record LoginUserCommand(UserCredentials Credentials) : IRequest<AuthResponse>;