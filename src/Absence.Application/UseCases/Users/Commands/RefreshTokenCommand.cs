using Absence.Application.UseCases.Users.DTOs;
using MediatR;

namespace Absence.Application.UseCases.Users.Commands;

public record RefreshTokenCommand(RefreshTokenRequest RefreshTokenRequest) : IRequest<AuthResponse>;