using Absence.Domain.Entities;
using Absence.Domain.Interfaces;
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
}