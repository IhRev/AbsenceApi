using Absence.Domain.Entities;

namespace Absence.Application.Identity;

public interface IRefreshTokenService
{
    Task<string> GenerateToken(UserEntity user, CancellationToken cancellationToken);

    bool Matches(UserEntity user, string refreshToken);
}