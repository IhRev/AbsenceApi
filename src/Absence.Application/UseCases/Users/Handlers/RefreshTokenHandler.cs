using Absence.Application.Identity;
using Absence.Application.UseCases.Users.Commands;
using Absence.Application.UseCases.Users.DTOs;
using MediatR;
using System.Security.Claims;

namespace Absence.Application.UseCases.Users.Handlers;

internal class RefreshTokenHandler(
    IUserService userService,
    IJwtService jwtService,
    IRefreshTokenService refreshTokenService
) : IRequestHandler<RefreshTokenCommand, AuthResponse>
{
    public async Task<AuthResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var principal = jwtService.GetPrincipalFromExpiredToken(request.RefreshTokenRequest.AccessToken);

        var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
        var userEntity = await userService.FindByIdAsync(userId);

        if (userEntity == null || 
            userEntity.RefreshToken != request.RefreshTokenRequest.RefreshToken ||
            userEntity.RefreshTokenExpiresAt <= DateTime.UtcNow)
        {
            return AuthResponse.Fail("Token is invalid");
        }

        var newAccessToken = jwtService.GenerateToken(userEntity);
        var newRefreshToken = await refreshTokenService.GenerateToken(userEntity, cancellationToken);

        return AuthResponse.Success(newAccessToken, newRefreshToken);
    }
}