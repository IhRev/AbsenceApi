using Absence.Domain.Entities;
using System.Security.Claims;

namespace Absence.Application.Identity;

public interface IJwtService
{
    string GenerateToken(UserEntity user);

    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
}