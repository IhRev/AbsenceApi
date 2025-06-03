using Absence.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Absence.Application.Identity;

public interface IUserService
{
    Task<bool> CheckPasswordAsync(UserEntity user, string password);

    Task<IdentityResult> CreateAsync(UserEntity user, string password);

    Task<UserEntity?> FindByEmailAsync(string email);

    Task<UserEntity?> FindByIdAsync(string id);

    Task UpdateAsync(UserEntity user);

    Task DeleteAsync(UserEntity user);

    Task<IdentityResult> ChangePasswordAsync(UserEntity user, string oldPassword, string newPassword);
}