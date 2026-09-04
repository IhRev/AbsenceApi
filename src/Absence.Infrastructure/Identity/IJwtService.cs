using Absence.Infrastructure.Entities;
using System.Security.Claims;

namespace Absence.Infrastructure.Identity;

public interface IJwtService
{
    string GenerateToken(UserEntity user);

    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}