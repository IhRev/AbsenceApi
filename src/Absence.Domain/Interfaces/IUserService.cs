using Absence.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Absence.Domain.Interfaces;

public interface IUserService
{
    Task<bool> CheckPasswordAsync(UserEntity user, string password);

    Task<IdentityResult> CreateAsync(UserEntity user, string password);

    Task<UserEntity?> FindByEmailAsync(string email);
}