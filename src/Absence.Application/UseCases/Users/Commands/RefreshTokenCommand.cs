using Absence.Application.UseCases.Users.DTOs;
using MediatR;

namespace Absence.Application.UseCases.Users.Commands;

public class RefreshTokenCommand(RefreshTokenRequest refreshTokenRequest) : IRequest<AuthResponse>
{
    public RefreshTokenRequest RefreshTokenRequest { get; } = refreshTokenRequest;
}