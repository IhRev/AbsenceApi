using Absence.Domain.Entities;
using Absence.Domain.Interfaces;
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
    private readonly IRandomGenerator _randomGenerator = randomGenerator;
    private readonly IUserService _userService = userService;

    public async Task<string> GenerateToken(UserEntity user, CancellationToken cancellationToken)
    {
        var token = Convert.ToBase64String(_randomGenerator.GenerateBytes(REFRESH_TOKEN_SIZE));

        await SaveToken(user, token, cancellationToken);

        return token;
    }

    private async Task SaveToken(UserEntity user, string token, CancellationToken cancellationToken)
    {
        user.RefreshToken = token;
        user.RefreshTokenExpires = DateTimeOffset.UtcNow.AddDays(_jwtConfiguration.RefreshTokenExpireTimeInDays);
        await _userService.UpdateAsync(user);
    }
}