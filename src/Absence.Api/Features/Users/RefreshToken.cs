using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Absence.Api.Common.Results;
using Absence.Infrastructure.Identity;
using MediatR;
using OneOf;

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
    public sealed class Command(RefreshTokenRequest refreshTokenRequest) : IRequest<OneOf<AuthTokens, BadRequest>>
    {
        public RefreshTokenRequest RefreshTokenRequest { get; } = refreshTokenRequest;
    }

    internal sealed class Handler(
        IUserService userService,
        IJwtService jwtService,
        IRefreshTokenService refreshTokenService
    ) : IRequestHandler<Command, OneOf<AuthTokens, BadRequest>>
    {
        public async Task<OneOf<AuthTokens, BadRequest>> Handle(Command request, CancellationToken cancellationToken)
        {
            var principal = jwtService.GetPrincipalFromExpiredToken(request.RefreshTokenRequest.AccessToken);
            if (principal is null)
            {
                return new BadRequest("Token is invalid");
            }

            var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userEntity = userId is null ? null : await userService.FindByIdAsync(userId);

            // A missing expiry must read as invalid: comparing null with <= yields false,
            // which would otherwise make such a token valid forever.
            if (userEntity is null ||
                !refreshTokenService.Matches(userEntity, request.RefreshTokenRequest.RefreshToken) ||
                userEntity.RefreshTokenExpiresAt is not { } expiresAt ||
                expiresAt <= DateTimeOffset.UtcNow)
            {
                return new BadRequest("Token is invalid");
            }

            return new AuthTokens
            {
                AccessToken = jwtService.GenerateToken(userEntity),
                RefreshToken = await refreshTokenService.GenerateToken(userEntity, cancellationToken)
            };
        }
    }
}
