using Absence.Infrastructure.Entities;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

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

    public bool Matches(UserEntity user, string refreshToken)
    {
        if (string.IsNullOrEmpty(user.RefreshToken) || string.IsNullOrEmpty(refreshToken))
        {
            return false;
        }

        try
        {
            var storedHash = Convert.FromHexString(user.RefreshToken);
            var presentedHash = SHA256.HashData(Encoding.UTF8.GetBytes(refreshToken));
            return CryptographicOperations.FixedTimeEquals(storedHash, presentedHash);
        }
        catch (FormatException)
        {
            return false;
        }
    }

    private async Task SaveToken(UserEntity user, string token, CancellationToken cancellationToken)
    {
        user.RefreshToken = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
        user.RefreshTokenExpiresAt = DateTimeOffset.UtcNow.AddDays(_jwtConfiguration.RefreshTokenExpireTimeInDays);
        await _userService.UpdateAsync(user);
    }
}