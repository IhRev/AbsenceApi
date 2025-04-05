using Absence.Domain.Entities;

namespace Absence.Domain.Interfaces;

public interface IJwtService
{
    string GenerateToken(UserEntity user);
}