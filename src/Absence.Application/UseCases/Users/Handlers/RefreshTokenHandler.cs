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
    private readonly IUserService _userService = userService;
    private readonly IJwtService _jwtService = jwtService;
    private readonly IRefreshTokenService _refreshTokenService = refreshTokenService;

    public async Task<AuthResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var principal = _jwtService.GetPrincipalFromExpiredToken(request.RefreshTokenRequest.AccessToken);

        var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
        var userEntity = await _userService.FindByIdAsync(userId);

        if (userEntity == null || 
            userEntity.RefreshToken != request.RefreshTokenRequest.RefreshToken ||
            userEntity.RefreshTokenExpiresAt <= DateTime.UtcNow)
        {
            return AuthResponse.Fail("Token is invalid");
        }

        var newAccessToken = _jwtService.GenerateToken(userEntity);
        var newRefreshToken = await _refreshTokenService.GenerateToken(userEntity, cancellationToken);

        return AuthResponse.Success(newAccessToken, newRefreshToken);
    }
}