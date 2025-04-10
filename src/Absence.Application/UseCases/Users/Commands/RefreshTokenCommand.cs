using Absence.Application.Common.DTOs;
using Absence.Application.Common.Responses;
using MediatR;

namespace Absence.Application.UseCases.Users.Commands;

public class RefreshTokenCommand(RefreshTokenRequest refreshTokenRequest) : IRequest<AuthResponse>
{
    public RefreshTokenRequest RefreshTokenRequest { get; } = refreshTokenRequest;
}