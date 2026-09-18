using Absence.Infrastructure.Entities;
using Microsoft.AspNetCore.Identity;

namespace Absence.Infrastructure.Identity;

public interface IUserService
{
    Task<bool> CheckPasswordAsync(UserEntity user, string password);

    Task<bool> IsLockedOutAsync(UserEntity user);

    Task AccessFailedAsync(UserEntity user);

    Task ResetAccessFailedCountAsync(UserEntity user);

    Task<IdentityResult> CreateAsync(UserEntity user, string password);

    Task<UserEntity?> FindByEmailAsync(string email);

    Task<UserEntity?> FindByIdAsync(string id);

    Task<IdentityResult> UpdateAsync(UserEntity user);

    Task<IdentityResult> DeleteAsync(UserEntity user);

    Task<IdentityResult> ChangePasswordAsync(UserEntity user, string oldPassword, string newPassword);
}