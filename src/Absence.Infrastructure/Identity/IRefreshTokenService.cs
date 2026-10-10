using Absence.Domain;

namespace Absence.Infrastructure.Identity;

public interface IRefreshTokenService
{
    Task<string> GenerateToken(UserEntity user, CancellationToken cancellationToken);

    bool Matches(UserEntity user, string refreshToken);
}