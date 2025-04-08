using Absence.Domain.Entities;

namespace Absence.Domain.Interfaces;

public interface IRefreshTokenService
{
    Task<string> GenerateToken(UserEntity user, CancellationToken cancellationToken);
}