using System.ComponentModel.DataAnnotations;
using Absence.Infrastructure.Identity;
using MediatR;
using System.Security.Claims;

namespace Absence.Api.Features.Users;

public class RefreshTokenRequest
{
    [Required]
    public required string AccessToken { get; set; }
    [Required]
    public required string RefreshToken { get; set; }
}

public static class RefreshToken
{
    public sealed class Command(RefreshTokenRequest refreshTokenRequest) : IRequest<AuthResponse>
    {
        public RefreshTokenRequest RefreshTokenRequest { get; } = refreshTokenRequest;
    }

    internal sealed class Handler(
        IUserService userService,
        IJwtService jwtService,
        IRefreshTokenService refreshTokenService
    ) : IRequestHandler<Command, AuthResponse>
    {
        private readonly IUserService _userService = userService;
        private readonly IJwtService _jwtService = jwtService;
        private readonly IRefreshTokenService _refreshTokenService = refreshTokenService;

        public async Task<AuthResponse> Handle(Command request, CancellationToken cancellationToken)
        {
            var principal = _jwtService.GetPrincipalFromExpiredToken(request.RefreshTokenRequest.AccessToken);
            if (principal is null)
            {
                return AuthResponse.Fail("Token is invalid");
            }

            var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userEntity = userId is null ? null : await _userService.FindByIdAsync(userId);

            if (userEntity == null ||
                !_refreshTokenService.Matches(userEntity, request.RefreshTokenRequest.RefreshToken) ||
                userEntity.RefreshTokenExpiresAt <= DateTime.UtcNow)
            {
                return AuthResponse.Fail("Token is invalid");
            }

            var newAccessToken = _jwtService.GenerateToken(userEntity);
            var newRefreshToken = await _refreshTokenService.GenerateToken(userEntity, cancellationToken);

            return AuthResponse.Success(newAccessToken, newRefreshToken);
        }
    }
}
