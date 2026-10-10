using Absence.Domain;
using Microsoft.AspNetCore.Identity;

namespace Absence.Infrastructure.Identity;

internal class UserService(UserManager<UserEntity> userManager) : IUserService
{
    public Task<IdentityResult> CreateAsync(UserEntity user, string password) => 
        userManager.CreateAsync(user, password);

    public Task<UserEntity?> FindByEmailAsync(string email) =>
        userManager.FindByEmailAsync(email);

    public Task<bool> CheckPasswordAsync(UserEntity user, string password) => 
        userManager.CheckPasswordAsync(user, password);

    public Task<bool> IsLockedOutAsync(UserEntity user) =>
        userManager.IsLockedOutAsync(user);

    public Task AccessFailedAsync(UserEntity user) =>
        userManager.AccessFailedAsync(user);

    public Task ResetAccessFailedCountAsync(UserEntity user) =>
        userManager.ResetAccessFailedCountAsync(user);

    public Task<UserEntity?> FindByIdAsync(string id) =>
        userManager.FindByIdAsync(id);

    public Task<IdentityResult> UpdateAsync(UserEntity user) =>
        userManager.UpdateAsync(user);

    public Task<IdentityResult> DeleteAsync(UserEntity user) =>
        userManager.DeleteAsync(user);

    public Task<IdentityResult> ChangePasswordAsync(UserEntity user, string oldPassword, string newPassword) =>
        userManager.ChangePasswordAsync(user, oldPassword, newPassword);
}