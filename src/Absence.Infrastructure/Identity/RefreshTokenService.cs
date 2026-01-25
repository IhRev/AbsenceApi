using Absence.Application.Common.Interfaces;
using Absence.Application.Identity;
using Absence.Domain.Entities;
using Microsoft.Extensions.Options;

namespace Absence.Infrastructure.Identity;

internal class RefreshTokenService(
    IOptions<JwtConfiguration> jwtConfiguration,
    IRandomGenerator randomGenerator,
    IUserService userService
) : IRefreshTokenService
{
    private const int REFRESH_TOKEN_SIZE = 64;
    private readonly JwtConfiguration _jwtConfiguration = jwtConfiguration.Value;

    public async Task<string> GenerateToken(UserEntity user, CancellationToken cancellationToken)
    {
        var token = Convert.ToBase64String(randomGenerator.GenerateBytes(REFRESH_TOKEN_SIZE));

        await SaveToken(user, token, cancellationToken);

        return token;
    }

    private async Task SaveToken(UserEntity user, string token, CancellationToken cancellationToken)
    {
        user.RefreshToken = token;
        user.RefreshTokenExpiresAt = DateTimeOffset.UtcNow.AddDays(_jwtConfiguration.RefreshTokenExpireTimeInDays);
        await userService.UpdateAsync(user);
    }
}