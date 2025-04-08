using Absence.Domain.Entities;
using Absence.Domain.Interfaces;
using Microsoft.Extensions.Options;

namespace Absence.Infrastructure.Identity;

internal class RefreshTokenService(
    IOptions<JwtConfiguration> jwtConfiguration,
    IRandomGenerator randomGenerator,
    IRepository<UserEntity> userRepository
) : IRefreshTokenService
{
    private const int REFRESH_TOKEN_SIZE = 64;
    private readonly JwtConfiguration _jwtConfiguration = jwtConfiguration.Value;
    private readonly IRandomGenerator _randomGenerator = randomGenerator;
    private readonly IRepository<UserEntity> _userRepository = userRepository;

    public async Task<string> GenerateToken(UserEntity user, CancellationToken cancellationToken)
    {
        var token = Convert.ToBase64String(_randomGenerator.GenerateBytes(REFRESH_TOKEN_SIZE));

        await SaveToken(user, token, cancellationToken);

        return token;
    }

    private async Task SaveToken(UserEntity user, string token, CancellationToken cancellationToken)
    {
        user.RefreshToken = token;
        user.RefreshTokenExpireTimeInDays = DateTimeOffset.UtcNow.AddDays(_jwtConfiguration.RefreshTokenExpireTimeInDays);

        await _userRepository.SaveAsync();
    }
}