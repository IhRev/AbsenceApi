using Absence.Infrastructure.Entities;
using Microsoft.AspNetCore.Identity;

namespace Absence.Infrastructure.Identity;

internal class UserService(UserManager<UserEntity> userManager) : IUserService
{
    private readonly UserManager<UserEntity> _userManager = userManager;

    public Task<IdentityResult> CreateAsync(UserEntity user, string password) => 
        _userManager.CreateAsync(user, password);

    public Task<UserEntity?> FindByEmailAsync(string email) =>
        _userManager.FindByEmailAsync(email);

    public Task<bool> CheckPasswordAsync(UserEntity user, string password) => 
        _userManager.CheckPasswordAsync(user, password);

    public Task<bool> IsLockedOutAsync(UserEntity user) =>
        _userManager.IsLockedOutAsync(user);

    public Task AccessFailedAsync(UserEntity user) =>
        _userManager.AccessFailedAsync(user);

    public Task ResetAccessFailedCountAsync(UserEntity user) =>
        _userManager.ResetAccessFailedCountAsync(user);

    public Task<UserEntity?> FindByIdAsync(string id) =>
        _userManager.FindByIdAsync(id);

    public Task UpdateAsync(UserEntity user) =>
        _userManager.UpdateAsync(user);

    public Task DeleteAsync(UserEntity user) =>
        _userManager.DeleteAsync(user);

    public Task<IdentityResult> ChangePasswordAsync(UserEntity user, string oldPassword, string newPassword) =>
        _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
}